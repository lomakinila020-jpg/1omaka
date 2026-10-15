using MSCLoader;
using UnityEngine;

namespace SatsumaStrobesMod
{
    public class SatsumaStrobesMod : Mod
    {
        public override string ID => "SatsumaStrobes";
        public override string Name => "Стробоскопы на капот и стекло";
        public override string Version => "1.2";
        public override string Author => "Твое Имя";

        private KeyCode toggleKey = KeyCode.U; 
        private bool isActive = false;

        private GameObject strobesParent;
        private Light[] leftLights;  
        private Light[] rightLights; 

        private float lastBlinkTime = 0f;
        private int currentStep = 0;

        public override void OnLoad()
        {
            ModConsole.Log("[SatsumaStrobes] Мод успешно загружен! Сели в Сатсуму -> Нажали U");
        }

        public override void Update()
        {
            if (CameraManager.Instance.GetCurrentCamera() == CameraManager.CameraType.Driving)
            {
                if (Input.GetKeyDown(toggleKey))
                {
                    isActive = !isActive;

                    if (isActive)
                    {
                        InitStrobes();
                        lastBlinkTime = Time.time;
                        currentStep = 0;
                    }
                    else
                    {
                        DestroyStrobes();
                    }
                }
            }

            if (isActive && strobesParent != null)
            {
                if (Time.time - lastBlinkTime >= 0.06f)
                {
                    lastBlinkTime = Time.time;
                    currentStep = (currentStep + 1) % 12;

                    if (currentStep == 0 || currentStep == 2 || currentStep == 4)
                    {
                        SetLightsState(leftLights, true);
                        SetLightsState(rightLights, false);
                    }
                    else if (currentStep == 6 || currentStep == 8 || currentStep == 10)
                    {
                        SetLightsState(leftLights, false);
                        SetLightsState(rightLights, true);
                    }
                    else
                    {
                        SetLightsState(leftLights, false);
                        SetLightsState(rightLights, false);
                    }
                }
            }
        }

        private void InitStrobes()
        {
            GameObject satsuma = GameObject.Find("SATSUMA(557hp)");
            if (satsuma == null) return;

            if (strobesParent != null) DestroyStrobes();

            strobesParent = new GameObject("SatsumaStrobesParent");
            strobesParent.transform.SetParent(satsuma.transform, false);

            leftLights = new Light;
            rightLights = new Light;

            leftLights = CreateLightObj("LeftHoodLight", new Vector3(-0.25f, 0.55f, 1.35f), Color.blue);
            leftLights = CreateLightObj("LeftGlassLight", new Vector3(-0.20f, 0.95f, 0.45f), Color.blue);

            rightLights = CreateLightObj("RightHoodLight", new Vector3(0.25f, 0.55f, 1.35f), Color.red);
            rightLights = CreateLightObj("RightGlassLight", new Vector3(0.20f, 0.95f, 0.45f), Color.red);
        }

        private Light CreateLightObj(string name, Vector3 localPos, Color lightColor)
        {
            GameObject lightObj = new GameObject(name);
            lightObj.transform.SetParent(strobesParent.transform, false);
            lightObj.transform.localPosition = localPos;
            lightObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Spot; 
            light.color = lightColor;
            light.range = 15f;          
            light.intensity = 18f;       
            light.spotAngle = 100f;      
            light.enabled = false;       

            GameObject visualCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visualCube.transform.SetParent(lightObj.transform, false);
            visualCube.transform.localScale = new Vector3(0.08f, 0.03f, 0.02f);
            
            Collider col = visualCube.GetComponent<Collider>();
            if (col != null) Object.Destroy(col); 
            
            Material mat = visualCube.GetComponent<Renderer>().material;
            mat.shader = Shader.Find("GUI/Text Shader"); 
            mat.color = lightColor;

            return light;
        }

        private void SetLightsState(Light[] lights, bool state)
        {
            if (lights == null) return;
            foreach (var light in lights)
            {
                if (light != null) light.enabled = state;
            }
        }

        private void DestroyStrobes()
        {
            if (strobesParent != null) 
            {
                Object.Destroy(strobesParent);
                strobesParent = null;
            }
            isActive = false;
        }
    }
}
