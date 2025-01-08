# SvaGUI: Immediate Mode GUI Rendering Assembly

<a href="https://github.com/svarog-engine/svagui/blob/main/svagui.jpg">
<img src="https://github.com/svarog-engine/svagui/blob/main/svagui.jpg">
</a>

**Proposal**: you implement only the primitive drawing functionalities (how to draw a rectangle, circle, text, an image, etc.), and any custom, complex controls or behaviours. Everything else is done and delimited into simple instructions that do one single thing and have well-defined IO behaviours.

### Example

To use SvaGUI from code, simply create a GUI object and run instructions. You can do normal code things in between instructions:

![image](https://github.com/user-attachments/assets/1f77e27f-1305-4ae7-b30f-4b3ec9733116)
<video src="https://github.com/user-attachments/assets/fa5126ac-5af3-4e9f-9279-bc2866a17308">

These instructions could be saved to a file, and all the custom code could be turned into a custom instruction, allowing full serialization.

The code above uses **contexts** to run branching code: these are like sub-procedures that your code can execute when needed, again, written completely as instructions:

![image](https://github.com/user-attachments/assets/62d2547e-d5fa-4f91-becd-254ef1a8023d)

For a fully custom instruction example, here's the debug print used above:

![image](https://github.com/user-attachments/assets/609001ff-8c23-43b2-897a-a5d419e4ee04)

Custom instructions can be called similarly to any other:

![image](https://github.com/user-attachments/assets/eacb0d85-b0f3-47b7-b0b5-5d6eae325649)
