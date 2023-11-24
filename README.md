# Destiny 2 API Load Splitter component for LiveSplit

Load Splitter component for LiveSplit allows to separate activity load time to a separate split. It will result in cumulative time alwayse precisely equeal to what you'd retrieve later in activity report from Bungie API

## tldr: 
* It's a Control component for LiveSplit
* Avoid keeping Splitter in Amber state ("waiting for activity start"). It is the most expensive application state and it must only be happening right before activity start
* Stop the Splitter component when not in use
* `Abandon activity` -> `Orbit` -> `Reset run` is preferred way of operation. Otherwise (loading into next run without leaving to orbit) you have to reset the LiveSplit run right after you leave activity and just before new one is started. Untested and may cause issues.

## Install

- Download the Server component from [GitHub repo releases](https://github.com/lildeadprince/LiveSplit.Destiny2.LoadSplitter/releases)
- Locate your LiveSplit installation directory
- Place the contents of the downloaded zip into the "<LiveSplit>/Components" directory

## Setup

Add the component to the Layout.
1. Right click on LiveSplit -> `Edit Layout...`
2. `+` -> `Control` -> `Destiny 2 Load Splitter`

In Layout Settings, you can change the Colors assigned to different Splitter states


### Control

* You **MUST** start the Splitter before programs can watch for Destiny activity start.
* Right click on LiveSplit -> `Control` -> `Start Load Splitter` ). 
* You **MUST** manually start it **EACH TIME** you launch LiveSplit.

## Contacts

- [@AlpenDitrix](https://discordapp.com/users/323887460813635585)
- /whisper AlpenDitrix#2247