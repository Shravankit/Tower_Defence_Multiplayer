using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.forward = _camera.transform.forward;
    }
}