# SE_Final_Project

## ESSENTIAL PART

### GRAPHIC ABSTRACTION

### PURPOSES

#### PROCESS TYPE

- This application applied **Agile** development process.

#### WHY WE CHOSE AGILE

- This game is a small-scale software, a *first-person 3D shooting game* for a wide market, and there are many competitors of the same type, and we need to produce it as soon as possible.

- At the same time, during the development process, we may *flexibly change requirements*. It is not appropriate to use the waterfall process that lacks flexibility.

- Nowadays, most software is developed using agile processes, and we follow the trend.

- We don't have too many regulations on the software, except for some fixed requirements, we decide the development strategy based on the current development process. Therefore, the agile process is a better choice for us.

#### POSSIBLE USAGE

Identify users
Hardcore single-player players: pursue high-difficulty level challenges and immersive story experience
Casual players: adapt to different skill levels through difficulty adjustment system (easy/medium/hard)
Potential cooperative mode players: reserve design interface for future multiplayer online functions

### SOFTWARE DEVELOPMENT PLAN
 #### Development Process

- For the **Requirement Engineering** phase:

  - Before starting this project, we used the Internet to do some research on the types of software currently used by people and analyzed the data of mainstream game platforms such as Steam and PSN. We found that the main audience of first-person shooter games is concentrated in the male player group. This type of players generally pursue the excitement during the fighting process, a variety of weapons, and scene designs in different eras and regions. At the same time, because casual players pay more attention to whether the game is easy to use and whether the game difficulty can be adjusted. Based on these findings, we determined the core requirements of the game as: to create an FPS game that integrates multiple combat elements (including melee, vehicle operation, science fiction settings, etc.), and to improve the playability and replay value of the game through 5 combat levels with different styles and a random prop drop system. For the general public, since there are many *competitors* in the game, *we need to seize the initiative quickly* and *gain market share*, and the market demand *changes rapidly* and customer needs are diverse, we adopt an agile software process model to quickly respond to player feedback

- For the **software design and implementation** phase:
  - For **defining the usage environment and mode of first-person shooter games**:
    - "Identify users": We identify the main users of the game. This may include hardcore single-player players,
casual players, and potential cooperative mode players, and determine their specific requirements.
    - Understand user goals and needs:
Identify user goals and needs in the context of the game. For example, individual users may want different game difficulties, exquisite weapon appearance. The team may focus on scene design, game fluency, etc. We collect and understand these goals to help us shape the features and functions of the system.
    - Consider usage scenarios (use cases): Create scenarios or use cases to describe how users play games in different situations.
Press "P" to open the pause menu
Press "W", "S", "A", "D" to control forward, backward, left and right movement
Press "TAB" to open the weapon backpack in some scenes
Press "E" to perform melee attacks in the pistol
Press "R" to change bullets
Press "SHIFT" to run
Press Left "CTRL" to crouch and walk quietly
Press "SPACE" to jump
Press the left mouse button to fire
Press the right mouse button to aim
In some scenes, press the "TAB" key to open the backpack and click the weapon you want to switch with the mouse or use the mouse wheel to switch weapons

    - Determine background factors: Consider background factors that may affect the use of the scheduling system.
Device constraints: Only supports PC (depends on keyboard and mouse operations, adjust DPI through MouseSensitivitySlider)
Physical environment: Brightness slider (LightSlider.value) adapts to different lighting conditions

  - For **define the system architecture**:
    - Identify system components: Split the software into multiple logical components (subsystems) that work together to meet functional requirements. For our game, common components may include *HUD display system*, *3D scene rendering system*, *player control system*, *enemy AI system*, *level flow system*, *data management system*
    - Choose an architecture pattern: For the first-person 3D shooting game, we choose a layered architecture pattern. This architecture divides the system into multiple layers, each responsible for a specific set of tasks. In our game, the layers may include *a presentation layer for user interface and 3D scene visualization*, *a logic layer for core game rules and behavior control*, and *a data layer responsible for data storage and management*.

#### Members
- Boty(P2320609):
  - role: project manager, software designer and software maintainer
  - responsibility:mainly determine the type of software and corresponding functions, write the source code for each of the function to make sure the software can work well and add relevant functions to the software to maintain its normal operation   
  - portion:
    - complete the some source code
    - complete some parts in the readme file
    - design the main structure of our software
    - do the testing for our software
    - do the maintenance work after the prototype of our software
- Lucas(P2320502):
  - role: project manager, software designer and software analyst
  - responsibility:mainly determine the type of software and corresponding functions, write the source code for each of the function to make sure the software can work well.
  - portion:
    - complete the remaining source code
    - do the testing for our software
    - design the main structure of our software
    - analyst the whole software 
    - improve the function of the software
- HUANG BAOYI(P2320324):
  - role: project manager, software tester and software analyst
  - responsibility:mainly determine the type of software and corresponding functions, test whether each of the function can work in diffierent cases then write a document to summary our software and analyze the rationality and consistency of the use of relevant functions  . 
  - portion:
    -  do the testing for our software
    -  design the main structure of our software
    -  analyst the whole software with each of the function
    -  complete remaining parts in the readme file

#### FUTURE PLAN

- **Integration with Other Tools**: Our game will integrate with other gaming platforms and tools, such as leaderboards, player statistics tracking, and social media sharing. These integrations aim to enhance player engagement and connectivity.
- **Online Battle Functionality**:
  - **Real-Time Multiplayer Battles**: Implement real-time multiplayer battles where players can compete against each other in various game modes.
  - **Matchmaking System**: Develop a robust matchmaking system that pairs players of similar skill levels to ensure balanced and competitive gameplay.
  - **In-Game Communication**: Add in-game communication features such as voice chat and text messaging to enhance teamwork and strategy during battles.
  - **Ranking and Rewards**: Introduce a ranking system and rewards for players based on their performance in online battles. This will incentivize players to improve their skills and compete in more matches.
  - **Regular Updates and Events**: Plan for regular updates and special events to keep the game fresh and engaging. This includes new battle modes, seasonal events, and limited-time challenges.
- **Security and Fair Play**: Implement robust anti-cheat mechanisms and monitoring systems to ensure fair play and maintain the integrity of the game.

This future plan aims to expand the game's features and provide a more engaging and competitive experience for players. By integrating online battle functionality and other enhancements, we strive to create a dynamic and enjoyable gaming environment.

## ADDITIONAL PART

### DOME
https://youtu.be/Q-jC71MjLw0
  - For **define the system architecture**:
    - Identify system components: Split the software into multiple logical components (subsystems) that work together to meet functional requirements. For our game, common components may include *HUD display system*, *3D scene rendering system*, *player control system*, *enemy AI system*, *level flow system*, *data management system*
    - Choose an architecture pattern: For the first-person 3D shooting game, we choose a layered architecture pattern. This architecture divides the system into multiple layers, each responsible for a specific set of tasks. In our game, the layers may include *a presentation layer for user interface and 3D scene visualization*, *a logic layer for core game rules and behavior control*, and *a data layer responsible for data storage and management*.

#### Members
- Boty(P2320609):
  - role: project manager, software designer and software maintainer
  - responsibility:mainly determine the type of software and corresponding functions, write the source code for each of the function to make sure the software can work well and add relevant functions to the software to maintain its normal operation   
  - portion:
    - complete the some source code
    - complete some parts in the readme file
    - design the main structure of our software
    - do the testing for our software
    - do the maintenance work after the prototype of our software
- Lucas(P2320502):
  - role: project manager, software designer and software analyst
  - responsibility:mainly determine the type of software and corresponding functions, write the source code for each of the function to make sure the software can work well.
  - portion:
    - complete the remaining source code
    - do the testing for our software
    - design the main structure of our software
    - analyst the whole software 
    - improve the function of the software
- HUANG BAOYI(P2320324):
  - role: project manager, software tester and software analyst
  - responsibility:mainly determine the type of software and corresponding functions, test whether each of the function can work in diffierent cases then write a document to summary our software and analyze the rationality and consistency of the use of relevant functions  . 
  - portion:
    -  do the testing for our software
    -  design the main structure of our software
    -  analyst the whole software with each of the function
    -  complete remaining parts in the readme file

#### FUTURE PLAN

- **Integration with Other Tools**: Our game will integrate with other gaming platforms and tools, such as leaderboards, player statistics tracking, and social media sharing. These integrations aim to enhance player engagement and connectivity.
- **Online Battle Functionality**:
  - **Real-Time Multiplayer Battles**: Implement real-time multiplayer battles where players can compete against each other in various game modes.
  - **Matchmaking System**: Develop a robust matchmaking system that pairs players of similar skill levels to ensure balanced and competitive gameplay.
  - **In-Game Communication**: Add in-game communication features such as voice chat and text messaging to enhance teamwork and strategy during battles.
  - **Ranking and Rewards**: Introduce a ranking system and rewards for players based on their performance in online battles. This will incentivize players to improve their skills and compete in more matches.
  - **Regular Updates and Events**: Plan for regular updates and special events to keep the game fresh and engaging. This includes new battle modes, seasonal events, and limited-time challenges.
- **Security and Fair Play**: Implement robust anti-cheat mechanisms and monitoring systems to ensure fair play and maintain the integrity of the game.

This future plan aims to expand the game's features and provide a more engaging and competitive experience for players. By integrating online battle functionality and other enhancements, we strive to create a dynamic and enjoyable gaming environment.

## ADDITIONAL PART

### DOME
-  Short demo: https://youtu.be/Q-jC71MjLw0
-  Complete demo: https://youtu.be/gDgfImw4XVs

### ENVIRONMENTS OF THE SOFTWARE DEVELOPMENT AND RUNNING
- Programming Language
   -  C#
- Development Environment
   -  Windows 11 64-bit, Unity 2020.3.18f1c1
- Minimum Computer Configurations
   -  CPU: i5-6500
   -  GPU: Nvidia GTX 1050
   -  Memory: 4G
   -  Display resolution: 1080p

### Declaration (Any open sources, packages which are not developed by you (yourgroup) should be declared clearly)
