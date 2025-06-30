#version 330 core
in vec2 aPosition;
in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 projection;
uniform mat4 model;

void main()
{
    gl_Position = projection * model * vec4(aPosition, 0.0, 1.0);
    texCoord = aTexCoord;
}
