-- Character Models
- In Blender select all faces to combine. Hit P and choose Selection
	- select armature and all objects in object mode
	- Export as .fbx
	- Use all Selected and visible objects
- Drag to Unity FBX file
- Select Rig, choose Humanoid, apply
- drag to scene, unpack, drag to Resources/Body
- drill down into prefab. Name body sections (Body_Skin, Body_Primary, etc.)
- Drag material (URP Lit) to all body sections
- Add Decal Projector to spine
- Add attachment points for accessories
- Add animator, controller and avatar 
- uncheck root motion
- Add AnimatorIKRelay



- Blender
-move object pivot to origin
 Select your object in Object Mode
 Go to Object menu → Set Origin → Origin to Geometry
