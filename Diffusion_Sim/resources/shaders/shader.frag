#version 330 core

in vec3 fragPos;
in vec3 fragNormal;
in vec2 texCoord;

in vec4 objColor;
in vec3 lightColor;
in vec3 lightPos;
in vec3 viewPos;

uniform sampler2D texture0;

out vec4 fragColor;

void main()
{
	// AMBIENT
	float ambientPow = 0.2;
	vec3 ambient = ambientPow * lightColor;
	
	// DIFFUSE
	vec3 norm = normalize(fragNormal);
	vec3 lightDir = normalize(lightPos - fragPos);
	float diff = max(dot(norm, lightDir), 0f);
	vec3 diffuse = diff * lightColor;
	
	// SPECULAR
	float specStr = 0.9f;
	vec3 viewDir = normalize(viewPos - fragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0f), 32);
	vec3 specular = specStr * spec * lightColor;
	
	// RESULT
	vec4 result = vec4(ambient + diffuse, 1f);
	vec4 texColor = texture(texture0, texCoord);
	if (texColor == vec4(0, 0, 0, 1))
	{
		fragColor = result * objColor;
	}
	else
	{
		fragColor = result * objColor * texColor;
	}
}