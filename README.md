# Genshin Team Manager

A Tool for viewing and compairing teams, and team pairs from Genshin Impact.

**The data required for using this tool must be collected manually, either by theorycrafting or using tools like [Genshin Optimizer](https://frzyc.github.io/genshin-optimizer/)**

(Glossary:) <br>
*DPM*: Damage per Minute, the average output the team deals in one minute <br>
*DPS*: Damage per Second, the average output the team deals in one second <br>
*Single Target*: Used for considering a situation where only one enemy is being fought <br>
*AoE*: Area of Effect, used for considering a situation where many enemies are being fought <br>

# Features

## Full Calc Page

### This is the page for viewing and compairing **Individual Teams**.

It will display a table of all of your registered teams, their members, and the DPM (or DPS) of the respective team in Single Target and in AoE. Can be sorted and searched.

![uTKdG0QTPM](https://github.com/user-attachments/assets/f56be58c-c863-40e6-9fad-29825bf779c9)

You can click on the name of a team to show its details, each team member along with their damage contribution to the total team output.

![image](https://github.com/user-attachments/assets/e28f5bd7-2633-4433-8105-efbc4fe5e8a6)

If you check the *Compare* checkbox, you can select another team to compare them. This shows in color which output is better or worse, and in percentage, how much better or worse it is.

![image](https://github.com/user-attachments/assets/68c09b27-1539-4f62-8709-8da1d150952a)


## Team Pair Page

### This is the page for viewing and comparing *Team Pairs*, as in, two teams that have no overlapping members, in a way that they can always be used in Spiral Abyss

It will display a table of all possible team pairings from the registered teams, along with their output in Single Target, AoE, and the average between the two. Can be sorted and searched.

![SeG9uOGxcV](https://github.com/user-attachments/assets/2dc59868-8be5-46b3-af5a-795992aa3378)

With the sort function, you can look up what is the theoretical best team pair for Abyss (disregarding enemies), if the abyss has more Single Target content, pick the pair with highest single target damage, if the abyss has more enemies in waves, pick the pair with highest AoE damage, if the abyss has both, pick the average.

You can also compare two pairs

![image](https://github.com/user-attachments/assets/f147a870-7f7e-4baa-b05b-612b21e5f830)

## Team Edit Page

### Pretty self explanatory, in this page you can add, edit, copy and delete teams. Also comes with a table with all teams so you can select it.

![image](https://github.com/user-attachments/assets/3b448690-96a5-42b8-837a-a23715cc78ed)

Changing the AoE value here will alter it only for this specific team. You can use this if due to constellation, team buff or play style, the aoe changes. Like when making a Xianyun plunge team.

When you try to add or change a character, you will be able to choose it from a grid, which is filterable and searchable.

![GMsGYuP2gc](https://github.com/user-attachments/assets/928b8554-242d-4061-a8fa-8fb0071b8475)

When you first add a character, it will use its default AoE multiplier and will use a "Raw" value of damage (an independant value). 

If you instead click the button labled "R" you will be brought to more details of that character.

![yYO1npHXvk](https://github.com/user-attachments/assets/9b768597-1f5b-409d-8c09-7163c3f011fc)

This way you can choose to use **relative damage**, as in, a *multiple of that value*. This can be used to cut off on manually changing the damage values for many teams, if you get an upgrade for that character **in a similar build**, changing the base value will also raise the damage in all teams that use that damage profile.
Additionally, Here you can change the global AoE (will change the AoE for every team that uses the default AoE value of this character)

## Additional Things

You can choose between DPM or DPS to be used throughout the tool.  
You can customize the image displayed for characters.  
If the tool is outdated, you can add missing characters manually.

---

This tool was created to enhance my knowledge in C# and MAUI Blazor Hybrid, being my first project using the latter.  
It also makes use of [Mudblazor](https://mudblazor.com/) and [Drag and Drop Polyfill](https://gist.github.com/iain-fraser/01d35885477f4e29a5a638364040d4f2).  

Credits to [wanderer.moe](https://wanderer.moe/) for the default character icons.


### Versions
  * 0.8.1 <br>
        - Fixed app freezing on dragging images <br>
        - You can now drag characters by their image in all pages except in the Team Edit page <br>
        - Pre-release of this version available <br>
  * 0.8 <br>
        - First added to GitHub. Release not yet available <br>


