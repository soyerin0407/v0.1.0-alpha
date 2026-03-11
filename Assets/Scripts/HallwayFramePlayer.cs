using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HallwayFramePlayer : MonoBehaviour
{
    [System.Serializable]
    public class FrameSection
    {
        public string sectionName;
        public float minY;
        public float maxY;
        public Sprite[] frames;
    }

    [Header("Display")]
    public Image hallwayImage;
    public TMP_Text debugText;

    [Header("Movement")]
    public float currentY = -11.5f;
    public float minY = -11.5f;
    public float maxY = 11.5f;
    public float moveSpeed = 2.0f;

    [Header("Sections")]
    public FrameSection[] sections;

    [Header("Turn Frames")]
    public Sprite[] frame1TurnRight;   // frame1t
    public Sprite[] frame4TurnLeft;    // frame4t
    public float turnFrameRate = 30f;

    private bool isTurning = false;
    private int currentFrameIndex = -1;

    void Update()
    {
        if (isTurning) return;

        HandleMovement();
        HandleTurnInput();
        UpdateHallwayFrame();
        UpdateDebugText();
    }

    void HandleMovement()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.W))
            input = 1f;
        else if (Input.GetKey(KeyCode.S))
            input = -1f;

        currentY += input * moveSpeed * Time.deltaTime;
        currentY = Mathf.Clamp(currentY, minY, maxY);
    }

    void HandleTurnInput()
    {
        FrameSection currentSection = GetCurrentSection();
        if (currentSection == null) return;

        // 첫 구간에서 오른쪽 보기
        if (currentSection.sectionName == "Frame1")
        {
            if (Input.GetKeyDown(KeyCode.D) && frame1TurnRight.Length > 0)
            {
                StartCoroutine(PlayTurnSequence(frame1TurnRight));
            }
        }

        // 네 번째 구간에서 왼쪽 보기
        if (currentSection.sectionName == "Frame4")
        {
            if (Input.GetKeyDown(KeyCode.A) && frame4TurnLeft.Length > 0)
            {
                StartCoroutine(PlayTurnSequence(frame4TurnLeft));
            }
        }
    }

    void UpdateHallwayFrame()
{
    if (hallwayImage == null)
    {
        Debug.Log("hallwayImage 연결 안 됨");
        return;
    }

    FrameSection section = GetCurrentSection();

    if (section == null)
    {
        Debug.Log("현재 구간 없음");
        return;
    }

    if (section.frames == null || section.frames.Length == 0)
    {
        Debug.Log("프레임 배열 비어 있음: " + section.sectionName);
        return;
    }

    float normalized = Mathf.InverseLerp(section.minY, section.maxY, currentY);
    int frameIndex = Mathf.RoundToInt(normalized * (section.frames.Length - 1));
    frameIndex = Mathf.Clamp(frameIndex, 0, section.frames.Length - 1);

    Debug.Log($"구간: {section.sectionName}, currentY: {currentY}, frameIndex: {frameIndex}");

    hallwayImage.sprite = section.frames[frameIndex];
}

    FrameSection GetCurrentSection()
    {
        foreach (FrameSection section in sections)
        {
            if (currentY >= section.minY && currentY <= section.maxY)
                return section;
        }

        return null;
    }

    void UpdateDebugText()
    {
        if (debugText == null) return;

        FrameSection section = GetCurrentSection();
        string sectionName = section != null ? section.sectionName : "None";

        debugText.text = $"Y: {currentY:F2}\nSection: {sectionName}\nFrame: {currentFrameIndex}";
    }

    System.Collections.IEnumerator PlayTurnSequence(Sprite[] turnFrames)
    {
        isTurning = true;

        float delay = 1f / turnFrameRate;

        for (int i = 0; i < turnFrames.Length; i++)
        {
            hallwayImage.sprite = turnFrames[i];
            yield return new WaitForSeconds(delay);
        }

        isTurning = false;
        currentFrameIndex = -1;
        UpdateHallwayFrame();
    }
    void Start()
    {
    UpdateHallwayFrame();
    }
}