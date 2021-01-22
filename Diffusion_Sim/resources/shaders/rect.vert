#version 330 core

// vertex
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec4 vColor;
layout (location = 3) in vec2 tCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform mat4 obj_translate;
uniform mat4 obj_scale;
uniform mat4 obj_rotate;

out vec3 fragPos;
out vec3 fragNormal;

out vec2 texCoord;

out vec4 objColor;

void main()
{
	//vec4 obj = vec4(vPosition, 1f) * obj_scale * obj_translate;
	//gl_Position = obj * model;
	gl_Position = vec4(vPosition, 1f) * obj_scale * model;
	
	fragPos = vec3(vec4(vPosition, 1f) * model);
	fragNormal = vNormal * mat3(transpose(inverse(model)));
	
	texCoord = tCoord;
	
	objColor = vColor;
}