# CS#Fixes

CS#Fixes is a CounterStrikeSharp plugin that fixes some bugs in Counter-Strike 2 and adds some commonly requested
features. This plugin is intended to replace CS2Fixes for servers that run CS# since CS2Fixes often conflicts with
CS# plugins. (Unlike cs2fixes, every feature in this plugin is optional and can be enabled or disabled via ConVars).

## Features

- Water Fix: Fixes being stuck to the floor underwater, allowing players to swim up.
- trigger_push Fix: Reverts trigger_push behaviour to that seen in CS:GO.
- CPhysBox Use Patch: Fixes CPhysBox use. Makes func_physbox pass itself as the caller in OnPlayerUse.
- No Block: Prevent players from blocking each other. (Sets debris collision on every player).
- Disable Subtick Movement: Disables sub-tick movement.
- Movement Unlocker: Enables movement unlocker.
- Force Full Alltalk: Enforces sv_full_alltalk 1.

## ConVars

- css_fixes_water_fix: Enable or disable the water fix. Default is 1.
- css_fixes_trigger_push_fix: Enable or disable the trigger_push fix. Default is 0.
- css_fixes_cphysbox_use_patch: Enable or disable the CPhysBox use patch. Default is 0.
- css_fixes_no_block: Enable or disable the no block feature. Default is 0.
- css_fixes_disable_subtick_movement: Enable or disable the disable subtick movement feature. Default is 0.
- css_fixes_movement_unlocker: Enable or disable the movement unlocker feature. Default is 0.
- css_fixes_force_full_alltalk: Enable or disable the force full alltalk feature. Default is 0.

## Why make this plugin when CS2Fixes already exists?

The primary motive for making this plugin was because I wanted the water fix from cs2fixes, but I couldn't run cs2fixes
on my server because it conflicted with other plugins. Specifically it consumed chat commands in a way that conflicted
with command handling in CS#. Additionally, I could have forked cs2fixes and kept it as a MetaMod plugin, but then I
would have had the problem of maintaining a C++ plugin for both windows and linux, whereas with CS# I can just compile
one plugin that is platform-agnostic, thus lowering the maintenance burden. Additionally, there are more developers
making plugins for CS# than there are for MetaMod, so I figured it would be easier for others to assist in maintaining
the plugin if it were written in C#. Also, it should be noted that unlike CS2Fixes, this plugin is not made for the
Zombie Escape gamemode, thus it doesn't have the features that are specific to that gamemode or that I felt should
probably be implemented in their own plugins like administrative features and ZE specific features.