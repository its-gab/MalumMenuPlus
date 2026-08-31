using UnityEngine;
using Il2CppSystem.Collections.Generic;
using MalumMenu.Network;
namespace MalumMenu;

public class KickUI : MonoBehaviour
{
    public static int windowHeight = 270;
    public static int windowWidth = 480;
    public static Rect windowRect;

    private Vector2 playerScrollPosition;

    private void Start()
    {
        windowRect = new Rect(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showKickMenu ||
            !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) ||
            MalumMenu.isPanicked)
            return;

        UIHelpers.ApplyUIColor();

        windowRect = GUI.Window(
            (int)WindowId.KickUI,
            windowRect,
            (GUI.WindowFunction)KickWindow,
            "Kick"
        );
    }

    private void KickWindow(int windowID)
    {
        if (!Utils.isShip)
        {
            GUI.DragWindow();
            return;
        }

        GUILayout.BeginVertical();

        // =========================
        // PLAYER SCROLL VIEW
        // =========================

        playerScrollPosition = GUILayout.BeginScrollView(
            playerScrollPosition,
            GUILayout.ExpandHeight(true)
        );

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player)
                continue;

            // Don't show host
            if (player.OwnerId == AmongUsClient.Instance.HostId)
                continue;

            // Don't show local player
            if (player == PlayerControl.LocalPlayer)
                continue;

            // Safety checks
            if (!player.Data)
                continue;

            if (string.IsNullOrEmpty(player.Data.PlayerName))
                continue;

            GUILayout.BeginHorizontal();

            // Player name
            GUILayout.Label(
                $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>",
                GUILayout.Width(180f)
            );

            GUILayout.FlexibleSpace();

            // =========================
            // COPY BUTTON
            // =========================

            if (GUILayout.Button(
                "Copy",
                GUIStylePreset.NormalButton,
                GUILayout.Width(60f)))
            {
                if (PlayerControl.LocalPlayer &&
                    player &&
                    player.Data)
                {
                    Utils.CopyPlayer(player);
                }
            }

            // =========================
            // KICK BUTTON
            // =========================

            if (GUILayout.Button(
                "Kick",
                GUIStylePreset.NormalButton,
                GUILayout.Width(60f)))
            {
                Utils.KickPlayer(player);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(2f);
        }

        GUILayout.EndScrollView();

        // =========================
        // SEPARATOR
        // =========================

        GUILayout.Box(
            "",
            GUIStylePreset.Separator,
            GUILayout.Height(1f),
            GUILayout.ExpandWidth(true)
        );

        GUILayout.Space(3f);

        // =========================
        // BOTTOM BUTTONS
        // =========================

        GUILayout.BeginHorizontal();

        // Kick All Crew
        if (GUILayout.Button(
            "Kick All Crew",
            GUIStylePreset.NormalButton))
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player)
                    continue;

                if (player.OwnerId == AmongUsClient.Instance.HostId ||
                    player == PlayerControl.LocalPlayer)
                    continue;

                if (!player.Data || !player.Data.Role)
                    continue;

                if (player.Data.Role.IsImpostor)
                    continue;

                Utils.KickPlayer(player);
            }
        }

        // Kick All Imps
        if (GUILayout.Button(
            "Kick All Imps",
            GUIStylePreset.NormalButton))
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player)
                    continue;

                if (player.OwnerId == AmongUsClient.Instance.HostId ||
                    player == PlayerControl.LocalPlayer)
                    continue;

                if (!player.Data || !player.Data.Role)
                    continue;

                if (!player.Data.Role.IsImpostor)
                    continue;

                Utils.KickPlayer(player);
            }
        }

        // Kick All
        if (GUILayout.Button(
            "Kick All",
            GUIStylePreset.NormalButton))
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player)
                    continue;

                if (player.OwnerId == AmongUsClient.Instance.HostId ||
                    player == PlayerControl.LocalPlayer)
                    continue;

                if (!player.Data)
                    continue;

                Utils.KickPlayer(player);
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        // Allow dragging the window
        GUI.DragWindow();
    }

    public void Update()
    {
        if (CheatToggles.showKickMenu &&
            (Input.GetKeyDown(KeyCode.K) ||
             Input.GetKeyDown(KeyCode.Keypad5)))
        {
            CheatToggles.showKickMenu = false;
        }
    }
}