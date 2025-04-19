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
- Boty (P2320609):
  - role: project manager, software designer and software maintainer
  - responsibility: mainly determine the type of software and corresponding functions, write the source code for each of the function to make sure the software can work well and add relevant functions to the software to maintain its normal operation   
  - portion:
    - complete the some source code
    - complete some parts in the readme file
    - design the main structure of our software
    - do the testing for our software
    - do the maintenance work after the prototype of our software
- Lucas (P2320502):
  - role: project manager, software designer and software analyst
  - responsibility: mainly determine the type of software and corresponding functions, write the source code for each of the function to make sure the software can work well.
  - portion:
    - complete the remaining source code
    - do the testing for our software
    - design the main structure of our software
    - analyst the whole software 
    - improve the function of the software
- HUANG BAOYI (P2320324):
  - role: project manager, software tester and software analyst
  - responsibility: mainly determine the type of software and corresponding functions, test whether each of the function can work in diffierent cases then write a document to summary our software and analyze the rationality and consistency of the use of relevant functions  . 
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

### Technical Implementation Principle
 #### 1. Main Character:
  - **1. Movement**
    - The Player (`gameObject`) is equipped with a `CharacterController` component. The `.Move()` method is used in the script to enable Player movement.
  - **2. Shooting**
    - A bullet firing point is set on the gun model. The `RaycastHit ray` detection method is used to shoot a ray from the bullet firing point. If the ray hits an enemy (tagged as 'enemy'), a bullet prefab is instantiated at the bullet firing point and a forward force is applied to the bullet prefab, launching the bullet. If the collider on the bullet contacts the collider on the enemy, it signifies that the bullet has hit the enemy, and the enemy will lose health. Each time a bullet is fired, the remaining bullet count decreases, and the `.text` method is used to assign the bullet count to the bullet count UI, which is displayed on the player's interface.
  - **3. Close Attack**
    - When the player uses a handgun and presses "E", they can use a dagger for close combat. In the script, if "E" is detected as being pressed, the `.SetTrigger()` method is used to call the close combat animation in the animation state machine. At the same time, the collider switch is set in the close combat animation. If the collider contacts the enemy's collider (tagged as 'enemy'), it signifies that the dagger has hit the enemy, and the enemy will lose health.
  - **4. Silent Walk**
    - "Walk Slower" When the script detects that the player is holding down the "CTRL" key, `crouchSpeed` is assigned to speed, thereby implementing "walk slower". At the same time, the `.Pause()` method is used to pause the walking or running sound effect, implementing "silent walk".
  - **5. HP Bar**
    - When the player takes damage from an enemy, the Health value decreases. In the script, the `.value` method is used to assign the Health value to the HP bar. If `playerHealthUIBar.value <= 0`, the player will die.
 #### 2. Game Loop (Win and Defeat):
   - In the game, if the player dies, or the vehicle the player is riding in is destroyed, or if the task is not completed within the specified time, the game will be defeated and the *Fail UI* will be displayed. If the player completes the specified task within the specified time without dying, the game will be won and the `Successful UI` will be displayed. If the *Fail UI* is displayed, the player must restart the current scene (Game must be a cycle).
 #### 3. Menu:
   - The main interface has three options: `Battle` (start button), `Setting` (option button), `Exit` (exit button). These buttons are button components in the UI type. When `Battle` is pressed, the `battleMode UI` will be displayed. When `Setting` is pressed, the `setting UI` will be displayed. When `Exit` is pressed, the `Application.Quit()` command will be executed to exit the game.
 #### 4. Enemy:
  - **1. There are six different enemies in the game:** Soldier with M4A1 (A), Soldier with AK47 (B), Armed Helicopter (C), Hummer (D), Paladin (E), Mutant (F)
    - A attacks by firing bullets with the M4A1 rifle.
    - B attacks by firing bullets with the AK47 rifle.
    - C attacks by launching missiles.
    - D attacks by firing bullets with a vehicle-mounted machine gun.
    - E attacks by slashing with a sword.
    - F attacks by performing a jump attack to create a fiery fissure.
  - **2. Each enemy has its own health bar and different health values**
    - A has a health of 100
    - B has a health of 100
    - C has a health of 700
    - D has a health of 3600
    - E has a health of 600
    - F has a health of 700.
  - **3. Enemy's health bar**
    - The enemy's health bar is placed above the enemy, and its remaining health is displayed on the health bar. Also, if the enemy is attacked by the player, the damage caused by the player will be displayed above the health bar.
  - **4. The enemy to disappear**
    - The `.value` method is used in the script to call the value of the health bar. If <= 0 , it signifies that the enemy is dead. The enemy's death animation will be played, and the `Destroy()` method will be called to make the enemy disappear after a certain period of time (A for 30 seconds, B for 5 or 30 seconds, C for 50 seconds, D for 20 seconds, E, F for 5 seconds).
  - **5.Timer**
    - There is a countdown (timer) at the top of the game interface. This countdown limits the time for the player to complete the task. If the player does not complete the specified task within this specified time, the game will fail and end. The countdown is controlled in the script using `leftTime - = Time.deltaTime`.
  - **6. Different Difficulties**
    - **1. Different difficulties (easy, normal, hard)**
      - The game's difficulty can be set in the `Setting UI` interface, which can be easy, normal, or hard.
    - **2. Each difficulties have obvious differences**
      - In the game, in scenes with an instantiation method to generate enemies (except `Battle 4`), the difficulty setting will change the number and speed of enemies generated and the damage value of enemy attacks. In scenes with fixed enemies, the difficulty setting will change the damage value of enemy attacks. The implementation method in the script is roughly as follows: After the player clicks the `Hard` Button (taking easy as an example), the following method is used to set variables related to difficulty:<br>
      &nbsp;&nbsp;&nbsp;&nbsp;`if (PlayerPrefs.GetString("Difficulties") == "Easy") {`<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`MAX_Damage = MAX_Damage *_ 0.7f;`<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`MIN_Damage = MIN_Damage * 0.7f;`<br>
      &nbsp;&nbsp;&nbsp;&nbsp;`}`<br>

## ADDITIONAL PART

### DOME
-  Short demo: https://youtu.be/Q-jC71MjLw0
-  Demonstration of the whole process: https://youtu.be/gDgfImw4XVs

### ENVIRONMENTS OF THE SOFTWARE DEVELOPMENT AND RUNNING
- Programming Language
   - C#
- Development Environment
   - Windows 11 64-bit
   - Unity 2020.3.18f1c1
- Minimum Computer Configurations
   - CPU: i5-6500
   - GPU: Nvidia GTX 1050
   - Memory: 4G
   - Display resolution: 1080p

### Declaration (Reference and packages we used)
 #### 1. Player moves, player shoots, switch weapons, create enemies
 - https://www.bilibili.com/video/BV1J8411d75s/?spm_id_from=333.1007.top_right_bar_window_default_collection.content.click&vd_source=01501ef4d710a82fa3ac8c1da1cfb795
 #### 2. PlayerPrefs data storage
 - https://blog.51cto.com/itMonon/5297873
 - https://blog.csdn.net/lxy20011125/article/details/130145944
 #### 3. Control the sound volume
 - https://www.niftyadmin.cn/n/4953277.html?action=onClick
 #### 4. Enemy instantiate
 - https://blog.csdn.net/qq_42453562/article/details/103808636
 #### 5. Add pictures/text
 - https://blog.csdn.net/WWeixq/article/details/134041700
 #### 6. Scene resources
 - https://assetstore.unity.com/packages/3d/environments/fantasy/low-poly-gladiators-arena- 167116
 - https://assetstore.unity.com/packages/2d/textures-materials/texture-pack-bricks-n-blocks- 180041
 - https://assetstore.unity.com/packages/3d/environments/urban/simple-city-pack-plain-100348
 - https://assetstore.unity.com/packages/3d/props/weapons/yughues-free-bombs-13147
 - https://assetstore.unity.com/packages/3d/environments/historic/roman-arena-14971
 - https://assetstore.unity.com/packages/3d/props/first-aid-set-160073
 - https://assetstore.unity.com/packages/3d/vehicles/land/low-poly-military-4x4-02-149610
 - https://assetstore.unity.com/packages/3d/environments/fantasy/lowpoly-village-road-forrunner-58102
 - https://assetstore.unity.com/packages/3d/environments/low-poly-rock-pack-57874
 - https://assetstore.unity.com/packages/3d/vehicles/air/helicopter-34383
 - https://assetstore.unity.com/packages/3d/environments/lowpoly-desert-town-178050
 - https://assetstore.unity.com/packages/3d/vehicles/air/helicopter-low-poly-264684
 #### 7. Script usage query
 - https://docs.unity3d.com/ScriptReference
 #### 8. Special problem solving
 - https://copilot.microsoft.com
