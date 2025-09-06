using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : PersistentSingleton<SessionManager>
{
    private ISession activeSession;

    public ISession ActiveSession
    {
        get => activeSession;
        set
        {
            activeSession = value;
            Debug.Log($"Active session: {activeSession}");
        }
    }

    const string playerNamePropertyKey = "playerName";

    public async Task InitializeAndSignIn()
    {
        try
        {
            if (!GameMultiplayerManager.playMultiplayer)
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync(); // Initialize Unity Gaming Services SDKs.
                }
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Anonymously authenticate the player
                    Debug.Log($"Sign in anonymously succeeded! PlayerID: {AuthenticationService.Instance.PlayerId}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    async Task<Dictionary<string, PlayerProperty>> GetPlayerProperties()
    {
        // Custom game-specific properties that apply to an individual player, ie: name, role, skill level, etc.
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey, playerNameProperty } };
    }

    public async Task StartSessionAsHost()
    {
        var playerProperties = await GetPlayerProperties();

        var options = new SessionOptions
        {
            MaxPlayers = 3,
            IsLocked = false,
            IsPrivate = false,
            PlayerProperties = playerProperties
        }.WithRelayNetwork(); // or WithDistributedAuthorityNetwork() to use Distributed Authority instead of Relay

        ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        Debug.Log($"Session {ActiveSession.Id} created! Join code: {ActiveSession.Code}");
    }

    public async Task JoinSessionById(string sessionId)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId);
        Debug.Log($"Session {ActiveSession.Id} joined!");
    }

    public async Task JoinSessionByCode(string sessionCode)
    {
        await InitializeAndSignIn(); // Ensure the player is signed in before joining a session
        StartCoroutine(WaitForInitAndSignIn());
        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode);
        Debug.Log($"Session {ActiveSession.Id} joined!");
    }

    async Task KickPlayer(string playerId)
    {
        if (!ActiveSession.IsHost) return;
        await ActiveSession.AsHost().RemovePlayerAsync(playerId);
    }

    async Task<IList<ISessionInfo>> QuerySessions()
    {
        var sessionQueryOptions = new QuerySessionsOptions();
        QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(sessionQueryOptions);
        return results.Sessions;
    }

    public async Task LeaveSession()
    {
        if (ActiveSession != null)
        {
            try
            {
                await ActiveSession.LeaveAsync();
                Debug.Log($"Left session {ActiveSession.Id} successfully.");
            }
            catch
            {
                // Ignored as we are exiting the game
            }
            finally
            {
                ActiveSession = null;
            }
        }
    }

    public void DisInitAndSignOut()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized) return;
        if (ActiveSession != null)
        {
            try
            {
                ActiveSession.LeaveAsync().Wait(); // Wait for the leave operation to complete
            }
            catch (Exception e)
            {
                Debug.LogError($"Error leaving session: {e.Message}");
            }
        }
        AuthenticationService.Instance.SignOut(); // Sign out the player
    }

    private IEnumerator WaitForInitAndSignIn()
    {        
        yield return new WaitUntil(() => UnityServices.State == ServicesInitializationState.Initialized);
    }
}