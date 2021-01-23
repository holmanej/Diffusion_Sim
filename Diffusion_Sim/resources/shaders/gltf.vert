#version 330 core

// vertex
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec4 vColor;
layout (location = 3) in vec2 tCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform int tCnt;
uniform mat4 Transforms[24];


//uniform mat4 obj_translate;
//uniform mat4 obj_scale;
//uniform mat4 obj_rotate;

out vec3 fragPos;
out vec3 fragNormal;

out vec2 texCoord;

out vec4 objColor;
out vec3 lightColor;
out vec3 lightPos;
out vec3 viewPos;

void main()
{
	vec4 vertexPos = vec4(vPosition, 1f);
	for (int i = 0; i < tCnt; i++)
	{
		vertexPos *= Transforms[3 * i] * Transforms[3 * i + 1] * Transforms[3 * i + 2];
	}
	
	gl_Position = vertexPos * model * view * projection;
	
	fragPos = vec3(vec4(vPosition, 1f) * model);
	fragNormal = vNormal * mat3(transpose(inverse(model)));
	
	texCoord = tCoord;
	
	objColor = vColor;
	lightColor = vec3(1f, 1f, 1f);
	lightPos = vec3(1000f, 1000f, -1000f);
	viewPos = vec3(0f, 0f, 0f);
}