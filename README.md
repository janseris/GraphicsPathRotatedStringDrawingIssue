# GraphicsPathRotatedStringDrawingIssue

~~black = expected, red = actual~~ (in previous commits where "sample text" is drawn)

mousemove = change drawing position
mouseclick left = start/stop rotation
mouseclick right = reset rotation to 0

Solved thanks to:
https://docs.microsoft.com/en-us/answers/questions/939696/graphicspath-text-rendering-issue-empty-space-betw.html

Solved issues (GraphicsPath string rendering offset issue only):
https://stackoverflow.com/questions/51006343/winforms-drawing-a-path-in-the-right-place

https://github.com/dotnet/winforms/issues/7485



Working result:

<video src="https://i.imgur.com/tDNsjcW.mp4"></video>

https://user-images.githubusercontent.com/64279914/180769968-3aec78e9-08b6-45d2-bd7e-0a3daa09f60f.mp4

