/*
    =============================================================================
    CS#Fixes
    Copyright (C) 2023-2024 Charles Barone <CharlesBarone> / hypnos <hyps.dev>
    =============================================================================

    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU General Public License, version 3.0, as published by the
    Free Software Foundation.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
    details.

    You should have received a copy of the GNU General Public License along with
    this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using CounterStrikeSharp.API.Modules.Cvars;

namespace CSSharpFixes;

public partial class CSSharpFixes
{
    public FakeConVar<bool> EnableWaterFix = new("css_fixes_water_fix", "Fixes being stuck to the floor underwater, allowing players to swim up.", true);
    public FakeConVar<bool> EnableTriggerPushFix = new("css_fixes_trigger_push_fix", "Reverts trigger_push behaviour to that seen in CS:GO.", true);
    public FakeConVar<bool> EnableCPhysBoxUseFix = new("css_fixes_cphys_box_use_fix", "Fixes CPhysBox use. Make func_physbox pass itself as the caller in OnPlayerUse.", false);
    //public FakeConVar<bool> EnableNavmeshLookupLagFix = new("css_fixes_navmesh_lookup_lag_fix", "Some maps with built navmeshes would cause tremendous lag.", false); // Commented out since it seems to cause crashes every time I test it...
    public FakeConVar<bool> EnableNoBlock = new("css_fixes_no_block", "Prevent players from blocking each other. (Sets debris collision on every player).", false);
    //public FakeConVar<bool> DisableTeamMessages = new("css_fixes_disable_team_messages", "Disables team join messages.", false); //TODO: NOT FINSIHED!
    public FakeConVar<bool> DisableSubTickMovement = new("css_fixes_disable_sub_tick_movement", "Disables sub-tick movement.", false);
    public FakeConVar<bool> EnableMovementUnlocker = new("css_fixes_enable_movement_unlocker", "Enables movement unlocker.", false);
    public FakeConVar<bool> EnforceFullAlltalk = new("css_fixes_enforce_full_alltalk", "Enforces sv_full_alltalk 1.", false);
    //public FakeConVar<bool> EnableEntityStringPurge = new("css_fixes_purge_entity_strings", "Enables purge of the EntityNames stringtable on new rounds", false); //TODO: NOT FINSIHED!
    
    private void RegisterConVars()
    {
        EnableWaterFix.ValueChanged += (sender, value) => { _configuration.EnableWaterFix = value; };
        EnableTriggerPushFix.ValueChanged += (sender, value) => { _configuration.EnableTriggerPushFix = value; };
        EnableCPhysBoxUseFix.ValueChanged += (sender, value) => { _configuration.EnableCPhysBoxUseFix = value; };
        //EnableNavmeshLookupLagFix.ValueChanged += (sender, value) => { _configuration.EnableNavmeshLookupLagFix = value; }; // Commented out since it seems to cause crashes every time I test it...
        EnableNoBlock.ValueChanged += (sender, value) => { _configuration.EnableNoBlock = value; };
        //DisableTeamMessages.ValueChanged += (sender, value) => { _configuration.DisableTeamMessages = value; }; //TODO: NOT FINSIHED!
        DisableSubTickMovement.ValueChanged += (sender, value) => { _configuration.DisableSubTickMovement = value; };
        EnableMovementUnlocker.ValueChanged += (sender, value) => { _configuration.EnableMovementUnlocker = value; };
        EnforceFullAlltalk.ValueChanged += (sender, value) => { _configuration.EnforceFullAlltalk = value; };
        //EnableEntityStringPurge.ValueChanged += (sender, value) => { _configuration.EnableEntityStringPurge = value; }; //TODO: NOT FINSIHED!
        
        RegisterFakeConVars(typeof(ConVar));
    }
}