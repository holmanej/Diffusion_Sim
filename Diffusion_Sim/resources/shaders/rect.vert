#version 330 core

// vertex
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec4 vColor;
layout (location = 3) in vec2 tCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform mat4 Transforms[24];
uniform int tCnt;

out vec3 fragPos;
out vec3 fragNormal;

out vec2 texCoord;

out vec4 objColor;

void main()
{
	vec4 vertexPos = vec4(vPosition, 1f);
	for (int i = 0; i < tCnt; i++)
	{
		vertexPos *= Transforms[3 * i] * Transforms[3 * i + 1] * Transforms[3 * i + 2];
	}
	
	gl_Position = vertexPos * model * view;// * projection;
	
	fragPos = vec3(vec4(vPosition, 1f) * model);
	fragNormal = vNormal * mat3(transpose(inverse(model)));
	
	texCoord = tCoord;
	
	objColor = vColor;
}