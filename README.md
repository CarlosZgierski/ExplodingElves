# ExplodingElves
This is Exploding Elved, created by Carlos Zgierski as a request from Fortis Games and their Exploding Elves Prompt.

This project was created using Unity vanilla everything, with no use of URP, ECS or any other framework beside the one that (for now) comes as standard with every Unity LTS version that can be downloaded from their archive (as of 2nd of April 2024).

I had some grandeur plans for this small project, but since I got Dengue Fever in the middle of the project, I had put aside some of those grandeur aspirations and stuck with the basics.

My main idea revolved on using Unity's navmesh technology with some really simple destination logic for the first challenge of this. It had gone through my mind creating a 2D "physics" based project as well, but I thought this approach would make for a more fun challenge and also for a better code.

All of the "Elfs" are created from the same prefab, using the same material with "GPU instancing" enabled, allowing for a big reduction in draw calls even with 4 different elf colors with the same material.

In the UI, the user can access multiple settings that impact the simulation, such as the spawn rate sliders, stopping and resuming the auto spawning and disabling the restriction of max possible elfs by color (which is not such a good idea taking into consideration that it may freeze the app if the numbers get too high).