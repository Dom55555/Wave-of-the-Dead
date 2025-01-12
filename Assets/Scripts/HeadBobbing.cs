using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public float bobbingSpeed = 14f;
    public float bobbingAmount = 0.028f;
    public float returnSpeed = 1.5f;
    public Transform arms;

    private float defaultCameraYPosition;
    private float defaultArmsYPosition;
    private float timer = 0f;

    void Start()
    {
        defaultCameraYPosition = transform.localPosition.y;
        defaultArmsYPosition = arms.localPosition.y;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            timer += Time.deltaTime * bobbingSpeed;
            float newY = defaultCameraYPosition + Mathf.Sin(timer) * bobbingAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
            newY = defaultArmsYPosition + Mathf.Sin(timer) * bobbingAmount*0.1f;
            arms.localPosition = new Vector3(arms.localPosition.x, newY, arms.localPosition.z); ;
        }
        else
        {
            timer = 0f;
            float smoothY = Mathf.Lerp(transform.localPosition.y, defaultCameraYPosition, Time.deltaTime * returnSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, smoothY, transform.localPosition.z);
            smoothY = Mathf.Lerp(arms.localPosition.y, defaultArmsYPosition, Time.deltaTime * returnSpeed*0.1f);
            arms.localPosition = new Vector3(arms.localPosition.x, smoothY, arms.localPosition.z);
        }
    }
    
}