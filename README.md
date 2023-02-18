# Outer Relics
![OUTER_RELICS_LOGO](/OuterRelics/Images/Main%20Logo%20Github.png)

Outer Relics is a mod for Outer Wilds that adds various Nomai Keys to the world which must be collected before the game can be completed.

Like most mods for Outer Wilds, this mod assumes that you have beaten the game before installing and reading this. There will be spoilers within this Readme. As Outer Wilds is best experienced with as few spoilers as possible, I urge you to click off this page and experience the game for yourself first before continuing if you have not already played it. There are no Echoes of the Eye spoilers in this readme.

## Introduction
![Ash Twin Project](/OuterRelics/Images/Ash%20Twin%20Project%202.png)

*As tensions and division arose between the Nomai during the construction of the Sun Station, concerns were raised over the security of the Ash Twin Project. While unlikely, it was possible for one to sneak into the Project and attempt to sabotage the Project's most precious component, the Advanced Warp Core. This would force the halting of the Project - and thus prevent the firing of the Sun Station. To prevent this, the ATP was set to lock itself in the event of disaster, and distribute 12 keys required to unlock it to trusted Nomai. But, when disaster did strike, when no Nomai were found, a bug in the system caused the Project to distribute the keys in random locations across the universe...*

Outer Relics adds up to 12 Nomai Keys in the Solar System, placed in random locations. The Ash Twin Project is locked down until all keys are found. The keys appear as holographic, glow, and give off a hum, so you'll know them when you see them. Collected keys are maintained across loops and are also saved.

If this sounds daunting, don't worry! A generous (and configurable) hint system exists to help you out. As you explore the Outer Wilds, you'll find question marks floating around that may tell you where a key can be found.

## Installation
![The sun rises over Timber Hearth](/OuterRelics/Images/Timber%20Hearth%20Sunrise.png)

Recommended: Simply find the mod in Outer Wilds Mod Manager and install it and its dependancies. To uninstall, simply disable the mod.

Manual: Download the latest release from the [releases page](https://github.com/GameWyrm/OuterRelics/releases). After unzipping, drag and drop the folder into %APPDATA%\OuterWildsModManager\OWML\Mods. You will also need to install [Menu Framework](https://github.com/misternebula/MenuFramework). To uninstall, simply remove those files from your mods folder. 

## How To Play
![A Key on Timber Hearth](/OuterRelics/Images/Key%20on%20Timber%20Hearth.png)

On the Main Menu, you'll notice a new button: "NEW OUTER RELICS RUN". If you're all right with default settings, you can start, but you should take a look at the mod config regardless. For information about the the configuration, check the [Settings](https://github.com/GameWyrm/OuterRelics/blob/Readme-Update/README.md#Settings) section.

Once you've selected your settings, go back to the Main Menu and click NEW OUTER RELICS RUN. You'll be prompted to insert a seed. If you leave it blank, you will be given a random seed. Two runs with the same settings and seed will be identical. After confirming your seed, the run will automatically start. To resume your run, simply Resume Expedition. 

To find keys, explore! They give off a loud hum, so you'll know if you're near one. There are twelve in total, and you can see which ones you have or do not have from the pause screen. Once you've found all twelve, a message will appear, informing you that the Ash Twin Project is now unlocked. You know what to do. A run isn't over until you you get to the end!

![Find Nomai Keys to beat the game. Find Hints to locate keys.](/OuterRelics/Images/KeyHintDisplay.png)

## Settings

Outer Relics is very customizable. You can tweak quite a few settings to match your taste:

* POOLS: These are where keys can appear. The first number after each name is the number of general locations controlled by this setting, while the second number is the number of unique spawn points. 

* SETTINGS: These are settings that affect the difficulty of your run.

  * ONE KEY MAX PER LOCATION: If this is on, keys are restricted from appearing in the same general area (note that certain larger locations, like the Nomai Cities, count as one greater area per district).

  * HINTS: Select the difficulty of hints. You can disable hints, make hints more vague, or make them more precise.

  * USELESS HINT CHANCE: There is a chance that hints will tell you something pointless instead of where a key is. As there are a lot of hints in the world, you can turn this up if you find runs too easy.

  * START OF LOOP HINTS: At the beginning of a loop, you'll be given a hint to the location of one key you don't have. You can set the number of loops that must pass before you start getting hints, or disable the feature entirely.

  * STAT SCREEN MUSIC: At the end of a run a stat screen will show up. This screen has custom music arranged by GameWyrm. If you don't like this music, you can disable it.

* ADVANCED: These are settings designed for users creating Outer Relics addons or attempting to fix bugs.

  * DEV MODE: This enables logging for Outer Relics, and enables tools for creating addons (check the [Creating Addons guide](https://github.com/GameWyrm/OuterRelics/wiki/Addon-Guide) for details)

  * MOD FOLDER NAME: If developing addons for Outer Relics, put the full unique name of your mod in this box.

  * DRY MODE: Starting a run generates a spoiler log but does not actually start a run or affect your save file.

## Compatibility
![Roasting Marshmallows with Riebeck](/OuterRelics/Images/Marshmallows%20with%20Riebeck.png)

Outer Relics is designed to be compatible with any mod, except mods that delete major objects in the base Hearthian Solar System, or mods that make significant changes to the Ash Twin Project.

You can use [New Horizons](https://github.com/Outer-Wilds-New-Horizons/new-horizons) with Outer Relics, provided the NH addons you are using do not delete major base system objects. Check for addons that support Outer Relics for more locations to search!

Outer Relics is compatible with [Outer Wilds Randomizer](https://github.com/PacificEngine/OW_Randomizer), and Randomizer is actually recommended for more interesting runs! Note that Outer Wilds Randomizer is not compatible with New Horizons.

## Addons
Outer Relics supports addons! You can add spawn points to any New Horizons mods that support Outer Relics. Check the Mod Manager to see what mods support Outer Relics.

If you want to create your own addons, check out the [Addon Guide](https://github.com/GameWyrm/OuterRelics/wiki/Addon-Guide).

## Feedback
If you run into a bug, or you want to suggest a new feature, [make an issue](https://github.com/GameWyrm/OuterRelics/issues) for it.

## Roadmap
![Overlooking Chert's Camp](/OuterRelics/Images/OverlookingChertsCamp.png)

Here are the features planned for the future:

-GOODIES: In addition to finding keys, you can also find goodies in the world! These provide bonuses to your abilities that will last for the duration of the run, such as extra jetpack power or more health.

-MULTIPLAYER: Once John Corby improves addon support for [Quantum Space Buddies](https://github.com/misternebula/quantum-space-buddies), I plan to add multiplayer so that you can hunt for keys with your friends! Hints, keys, and goodies will be shared.

-HINT LOGGING: Currently, you'll have to keep your own notes for where keys are, but I plan to eventually use Custom Ship Log Modes to create a menu that keeps track of what hints you have gotten.

-METROIDVANIA MODE: Finding keys is fun and all, but what if there were MORE arbitrary blockades in your way? Tornadoes, Quantum Rules, your jetpack, and more randomized!

-ARCHIPELAGO SUPPORT: While it would be cool to add Outer Relics support for the Story Jam mod, I'm actually talking about the cross-game multiworld randomizer system, Archipelago. Other people can find your items, and you can find theirs in Outer Wilds.
