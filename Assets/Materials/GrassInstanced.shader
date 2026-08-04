Shader "Custom/GrassLitBillboard" {
    Properties {
        _MainTex ("Grass Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        // DisableBatching потрібен, щоб Unity не ламала координати при спробі об'єднати меші
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
            "DisableBatching"="True"
        }

        // Вимикаємо відсікання задньої грані
        Cull Off
        LOD 200

        CGPROGRAM
        // Standard - фізично коректне освітлення
        // addshadow - змушує Unity правильно рахувати тіні для нашої повернутої трави
        // vertex:vert - кажемо Unity використати нашу функцію для зміни геометрії
        #pragma surface surf Standard addshadow vertex:vert alphatest:_Cutoff
        #pragma multi_compile_instancing

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void vert (inout appdata_full v) {
            UNITY_SETUP_INSTANCE_ID(v);

            // 1. Отримуємо світову позицію центру травинки
            float3 centerWorldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;

            // 2. Рахуємо вектор від травинки до камери
            float3 forward = _WorldSpaceCameraPos - centerWorldPos;
            forward.y = 0; // Трава крутиться тільки навколо осі Y (щоб не лягала на землю)
            forward = normalize(forward);

            // 3. Будуємо систему координат для повороту (вектор "вправо" та "вгору")
            float3 up = float3(0, 1, 0);
            float3 right = normalize(cross(up, forward));

            // 4. Витягуємо масштаб (scale), який ми задавали в C# скрипті (Vector3 randomScale)
            float2 scale = float2(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y))
            );

            // 5. Вираховуємо нові світові координати кожної вершини Quad'а
            // v.vertex.x - це відхилення вершини вліво/вправо, v.vertex.y - вгору/вниз
            float3 newWorldPos = centerWorldPos + right * (v.vertex.x * scale.x) + up * (v.vertex.y * scale.y);

            // 6. Повертаємо координати назад у локальний простір (Unity очікує їх там)
            v.vertex.xyz = mul(unity_WorldToObject, float4(newWorldPos, 1)).xyz;

            // 7. Перераховуємо нормалі, щоб освітлення падало правильно з боку камери
            v.normal = mul((float3x3)unity_WorldToObject, forward);
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Читаємо текстуру
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

            // Передаємо колір та прозорість у стандартний рушій освітлення Unity
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    // FallBack потрібен для того, щоб трава могла відкидати тіні на інші об'єкти
    FallBack "Transparent/Cutout/Diffuse"
}