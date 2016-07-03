using UnityEngine;
using System.Net;

public class RecordsManager : MonoBehaviour {
    public const int MAX_RECORDS_PER_BOX = 8;

    public GameObject recordTemplate;
    public GameObject[] boxes;
    public CameraManager cam;

    private Record[] records;
    private int amountOfRecords;
    private int currentlySelectedRecord = 0;
    private int[] savedSelectedRecords;
    private int currentlySelectedBox = 0;

    private float scrollTime = 0;
    private bool selected = false;
    private bool canScrollRecords = true;
    private bool canScrollBoxes = true;
    private bool canSelect = true;
    private bool canSpin = true;

    void Start() {
        RecordInfo[] library = RecordInfo.LoadUserLibrary(boxes.Length * MAX_RECORDS_PER_BOX);
        
        savedSelectedRecords = new int[boxes.Length];
        records = new Record[MAX_RECORDS_PER_BOX * boxes.Length];
        LoadRecordLibrary(library);

        currentlySelectedRecord = RecordsInBox(0) - 1;
        for (int i = 1; i < boxes.Length; i++) {
            savedSelectedRecords[i] = RecordsInBox(i) - 1;
        }
    }
	
	void Update () {
        if (Input.GetAxis("Vertical") == 0 || (Input.GetAxis("Vertical") != 0 && Time.fixedTime - scrollTime > 0.5f)) {
            canScrollRecords = true;
        }
        if (Input.GetAxis("Horizontal") == 0 || (Input.GetAxis("Horizontal") != 0 && Time.fixedTime - scrollTime > 0.5f)) {
            canScrollBoxes = true;
        }
        int scrollAmount = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10 - Input.GetAxis("Vertical"));
        if (scrollAmount != 0 && canScrollRecords && !selected) {
            canScrollRecords = false;
            scrollTime = Time.fixedTime;
            ScrollRecords(scrollAmount > 0);
        }
        float boxScrollAmount = Input.GetAxis("Horizontal");
        if (boxScrollAmount != 0 && canScrollBoxes && !selected) {
            canScrollBoxes = false;
            scrollTime = Time.fixedTime;
            savedSelectedRecords[currentlySelectedBox] = currentlySelectedRecord;
            ScrollBoxes(boxScrollAmount < 0);
            currentlySelectedRecord = savedSelectedRecords[currentlySelectedBox];
        }

        if (!Input.GetButton("Action (Primary)")) {
            canSelect = true;
        }
        if (Input.GetButton("Action (Primary)") && canSelect) {
            if (selected) {
                Unselect();
            } else {
                Select();
            }
            canSelect = false;
        }
        
        if (!Input.GetButton("Action (Secondary)")) {
            canSpin = true;
        }
        if (Input.GetButton("Action (Secondary)") && canSpin && selected) {
            Record selectedRecord = GetRecordAt(currentlySelectedRecord);
            if (selectedRecord != null) {
                selectedRecord.Flip();
            }
            canSpin = false;
        }
    }

    void Select () {
        Record selectedRecord = GetRecordAt(currentlySelectedRecord);
        if (selectedRecord != null) {
            selected = true;
            SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 500, 0);
            selectedRecord.SetSelected(true);
            cam.targetRotation.x = 0;
            cam.targetFov = 47 + 23 * ((float)currentlySelectedRecord / MAX_RECORDS_PER_BOX);
        }
    }

    void Unselect () {
        Record selectedRecord = GetRecordAt(currentlySelectedRecord);
        if (selectedRecord != null) {
            if (selected) {
                SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 30, 50);
            } else {
                SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 20, 50);
            }
            selected = false;
            selectedRecord.SetSelected(false);
            cam.targetRotation.x = 40;
            cam.targetFov = 80;
        }
    }

    void ScrollRecords(bool backwards = false, int iterations = 1) {
        int previouslySelected = currentlySelectedRecord;
        int boxSize = RecordsInBox(currentlySelectedBox);
        currentlySelectedRecord = Mathf.Clamp(currentlySelectedRecord + (backwards ? 1 : -1), 0, boxSize - 1);
        if (previouslySelected != currentlySelectedRecord) {
            int direction = previouslySelected > currentlySelectedRecord ? -1 : 1;
            Record previous = GetRecordAt(previouslySelected);
            if (previous != null) {
                SetSpringValue(previous.GetComponent<HingeJoint>(), 20, 50 * direction);
                previous.SetSelected(false);
            }
            
            Unselect();
        }

        // Do the whole thing again if there are iterations and space
        if (currentlySelectedRecord > 0 && currentlySelectedRecord < boxSize - 1 && iterations > 1) {
            ScrollRecords(backwards, iterations - 1);
        }
    }

    void ScrollBoxes (bool backwards = false, int iterations = 0) {
        int previouslySelected = currentlySelectedBox;
        int tempCurrentlySelected = Mathf.Clamp(currentlySelectedBox + (backwards ? -1 : 1), 0, boxes.Length - 1);
        if (previouslySelected != tempCurrentlySelected) {
            // Unselect the record in the current box before changing boxes
            Unselect();
            currentlySelectedBox = tempCurrentlySelected;
            // Make camera's target something proper
            Vector3 deltaPos = boxes[currentlySelectedBox].transform.position - boxes[previouslySelected].transform.position;
            cam.targetPosition += deltaPos;
        }

        // Do the whole thing again if there are iterations and space
        if (currentlySelectedBox > 0 && currentlySelectedBox < boxes.Length - 1 && iterations > 0) {
            ScrollBoxes(backwards, iterations - 1);
        }
    }

    void SetSpringValue(HingeJoint joint, float spring, float targetPosition) {
        JointSpring tempSpring = joint.spring;
        tempSpring.spring = spring;
        tempSpring.targetPosition = targetPosition;
        joint.spring = tempSpring;
    }

    Record GetRecordAt (int index) {
        return GetRecordAt (index, currentlySelectedBox);
    }

    Record GetRecordAt (int index, int boxIndex) {
        return records[(Mathf.Clamp(index, 0, MAX_RECORDS_PER_BOX - 1) + boxIndex * MAX_RECORDS_PER_BOX) % (MAX_RECORDS_PER_BOX * boxes.Length)];
    }

    int RecordsInBox (int boxIndex) {
        int total = 0;
        for (int i = 0; i < MAX_RECORDS_PER_BOX; i++) {
            if (GetRecordAt(i, boxIndex) != null) {
                total++;
            }
        }
        return total;
    }

    void LoadRecordLibrary (RecordInfo[] infos) {
        foreach (RecordInfo info in infos) {
            CreateRecord(info);
        }
    }

    void CreateRecord (RecordInfo info) {
        if (amountOfRecords >= MAX_RECORDS_PER_BOX * boxes.Length) {
            Debug.LogError("Tried to add record '" + info.GetFullName() + "' to a full box!");
        } else {
            Transform box = boxes[amountOfRecords / MAX_RECORDS_PER_BOX].transform;

            float recordOffset = ((float)(amountOfRecords % MAX_RECORDS_PER_BOX) / MAX_RECORDS_PER_BOX) * 0.8f - 0.4f;
            Vector3 positionOffset = new Vector3(recordOffset, 0, 0);

            GameObject newRecordObject = (GameObject)Instantiate(recordTemplate, box.position + positionOffset, recordTemplate.transform.rotation);
            newRecordObject.transform.parent = box;
            newRecordObject.name = "Record #" + amountOfRecords;

            Record newRecord = newRecordObject.GetComponent<Record>();
            newRecord.info = info;
            records[amountOfRecords++] = newRecord;
        }
    }
}
