# tcl-data-aggregator
---
## Overview

The TCL Data Aggregator is a simple executable tool that compiles data for the ongoing League of Legends league, Tilt Champions League. This tool uses the [Riot Games API](https://developer.riotgames.com/) to compile deep statistics on TCL matches, allowing folks to analyze and aggregate the data into meaningful statistics for infographics, practice, P/B strategies, etc. 

In principle, the tool takes in a collection of matches that were played in TCL (as input), and outputs:
- A spreadsheet of TCL data, with one sheet for overall team stats, and one sheet for overall player stats.
- A .txt file representing a set of fun aggregated stats across the set of input matches.

## Process

Due to the nature of the Riot Games LoL API, a unique process was needed to properly aggregate data and attribute the data to players and teams. In summary, *Custom Games* played through the League of Legends client are automatically scrubbed of all player-identifying info - most notably summoner names, and *Custom Games* have no concept of teams and team names.

Because of this, conceptually, this tool takes in a set of metadata, manually inputted by TCL admins. The goal of this metadata is to provide necessary information around each match, so that this tool can pair that data up with game stats, finishing with a massaged data set that can pair up things like "Week/Day" and "Team Name" with game stats of what happened within the match. 

The full process is defined as follows:
1. User provides a metadata excel file ([Template can be found here](https://github.com/Abarn279/tcl-data-aggregator/blob/master/Data/TCL_Metadata.xlsx))
2. Tool imports the metadata, ensuring it's properly sorted by week/day/game, which will control the final output order
3. Tool iterates through each metadata record, calling the Riot API for each, and creating a final data record that's combination of metadata + riot api data. 
4. Tool caches final set of data in a local file (cache.dat). **Every time the tool is rerun, this cache will be used for the data pull in place of step 3**.
5. Tool outputs all data into multiple excel sheets with proper visualization.
6. Tool uses data set to aggregate fun stats data, outputs to a text file.

