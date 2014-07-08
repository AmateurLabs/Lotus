using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Lotus {
    public class Shader : IDisposable {

        static string defaultVertexShader = 
@"
#version 120

varying vec4 diffuse,ambient;
varying vec3 normal,halfVector;
 
void main()
{
    /* first transform the normal into eye space and
    normalize the result */
    normal = normalize(gl_NormalMatrix * gl_Normal);
 
    /* pass the halfVector to the fragment shader */
    halfVector = gl_LightSource[0].halfVector.xyz;
 
    /* Compute the diffuse, ambient and globalAmbient terms */
    diffuse = gl_Color * gl_LightSource[0].diffuse;
    ambient = gl_Color * gl_LightSource[0].ambient;
    gl_Position = ftransform();
 
}";

        static string defaultFragmentShader =
@"#version 120

varying vec4 diffuse,ambient;
varying vec3 normal,halfVector;
 
void main()
{
    vec3 n,halfV,lightDir;
    float NdotL,NdotHV;
 
    lightDir = vec3(gl_LightSource[0].position);
 
    /* The ambient term will always be present */
    vec4 color = ambient;
    /* a fragment shader can't write a varying variable, hence we need
    a new variable to store the normalized interpolated normal */
    n = normalize(normal);
    /* compute the dot product between normal and ldir */
    
    NdotL = max(dot(n,lightDir),0.0);
    if (NdotL > 0.0) {
        color += diffuse * NdotL;
        halfV = normalize(halfVector);
        NdotHV = max(dot(n,halfV),0.0);
        color += gl_FrontMaterial.specular *
                gl_LightSource[0].specular *
                pow(NdotHV, gl_FrontMaterial.shininess);
    }
 
    gl_FragColor = color; 
}";

        int vertexShaderHandle,
            fragmentShaderHandle,
            shaderProgramHandle;

        public Shader() {
            CreateShaders(defaultVertexShader, defaultFragmentShader);
        }

        bool disposed;

        public ~Shader() {
            if (!disposed) Dispose();
        }

        public Shader(string vertexShaderSource, string fragmentShaderSource) {
            CreateShaders(vertexShaderSource, fragmentShaderSource);
        }

        void CreateShaders(string vertexShaderSource, string fragmentShaderSource) {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            // Create program
            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(shaderProgramHandle);
        }

        public void Draw() {
            Camera.Current.CurrentShader = this;
            GL.UseProgram(shaderProgramHandle);
        }

        public static void Reset() {
            GL.UseProgram(0);
        }
        
        public void Dispose() {
            disposed = true;
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
            GL.DeleteProgram(shaderProgramHandle);
        }
    }
}
