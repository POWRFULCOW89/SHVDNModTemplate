using GTA;
using GTA.Math;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SHVDNModTemplate
{

    public sealed class Mod : Script
    {
        private readonly ScriptSettings config = ScriptSettings.Load("scripts\\ModSettings.ini");
        private readonly List<Vehicle> vehicles = new List<Vehicle>();
        private readonly ObjectPool pool = new ObjectPool();
        private readonly NativeMenu menu = new NativeMenu("Mod", "Menu");
        private readonly NativeItem item = new NativeItem("Spawn car", "Spawn a car", "A really fast one");

        private void ClearPools()
        {
            foreach (var v in vehicles)
            {
                v.MarkAsNoLongerNeeded();
            }
        }

        private void ItemActivated(object sender, EventArgs e)
        {
            var v = World.CreateVehicle(VehicleHash.Adder, World.GetNextPositionOnStreet(Game.Player.Character.Position));
            vehicles.Add(v);
            menu.Visible = false;
        }

        private void CreateMenu()
        {
            item.Activated += ItemActivated;
            menu.Add(item);
            pool.Add(menu);
        }

        private void Setup()
        {
            CreateMenu();

            var modName = config.GetValue("Config", "ModName", "Mod");
            var showHelp = config.GetValue("Config", "ShowHelp", true);

            if (showHelp) Notification.Show($"Mod loaded: {modName}");

            GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to open a menu.", 10);
        }

        public Mod()
        {
            Setup();
            SetupPlayer();

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAbort;
        }

        public void OnTick(object sender, EventArgs e)
        {
            pool.Process();

            if (Game.IsControlJustPressed(GTA.Control.Context))
            {
                menu.Visible = true;
            }

            MainLoop();
        }

        private static void SetupPlayer()
        {
            Game.Player.IsInvincible = true;
        }

        private static void ResetPlayer()
        {
            Game.Player.IsInvincible = false;
        }

        private static void MainLoop()
        {
            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(1, 1, 1), Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), Color.Yellow);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.T:
                    if (!Game.IsWaypointActive) return;
                    Game.Player.Character.Position = (World.WaypointPosition + Vector3.WorldUp * 3);
                    break;
                case Keys.P:
                    Notification.Show(Game.Player.Character.Position.ToString());
                    break;
            }
        }

        private void OnAbort(object sender, EventArgs e)
        {
            ResetPlayer();
            ClearPools();
        }
    }
}
