# Organica Wild

## How to use in a project

Go to the package manager inside an existing unity project and add the package via git url.
Import the samples you would like to use with the import buttons in the package manager.

## How to develop and change the package code

1. Create an empty project
2. Clone the project into the Packages/ folder of the Unity Project
3. Unity will import the package 
4. Change the code
5. Commit and push your changes

### How to change the samples

The samples folder is not visible inside unity. 
For the samples to be detected in the Package Manager the samples folder must be named Samples~.
However to edit the content simply remove the tilde.
After the changes are made re-add the tilde. 
If you forget this step the samples are not shown in the package manager when the package is added.


