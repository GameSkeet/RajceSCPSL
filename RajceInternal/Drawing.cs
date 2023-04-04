using System;
using UnityEngine;

namespace RajceInternal
{
    internal class Drawing
    {
        private static Material lineMaterial;

        static Drawing()
        {
            lineMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {   BindChannels { Bind \"Color\",color }   Blend SrcAlpha OneMinusSrcAlpha   ZWrite Off Cull Off Fog { Mode Off }} } }");
            lineMaterial.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
            lineMaterial.shader.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
        }

        public static void DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            lineMaterial.SetPass(0);

            GL.Begin(GL.LINES);
            GL.Color(color);

            GL.Vertex(p1.ToV3());
            GL.Vertex(p2.ToV3());

            GL.End();
        }
    }
}
