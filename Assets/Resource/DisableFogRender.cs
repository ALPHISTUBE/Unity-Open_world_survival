using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DisableFogRender : MonoBehaviour
{
    private Camera cam;

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += BeginRender;
        RenderPipelineManager.endCameraRendering += EndRender;
    }
    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= BeginRender;
        RenderPipelineManager.endCameraRendering -= EndRender;
    }

    void BeginRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera.name == this.name)
        {
            RenderSettings.fog = false;
        }

    }

    void EndRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera.name == this.name)
        {
            RenderSettings.fog = true;
        }
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
        StartCoroutine(DisableCamera());
    }

    IEnumerator DisableCamera()
    {
        cam.enabled = true;
        cam.Render();
        yield return new WaitForSeconds(.3f);
        cam.enabled = false;
        StartCoroutine(DisableCamera());
    }
}