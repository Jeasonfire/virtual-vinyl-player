using UnityEngine;

public class RecordsManager : MonoBehaviour {
    public const int MAX_RECORDS_PER_BOX = 20;

    public GameObject recordTemplate;
    public GameObject[] boxes;
    public CameraManager cam;

    private Record[] records;
    private int amountOfRecords;
    private int currentlySelectedRecord = 0;
    private int[] savedSelectedRecords;
    private int currentlySelectedBox = 0;

    private float scrollTime = 0;
    private bool canScrollRecords = true;
    private bool canScrollBoxes = true;

    void Start() {
        RecordInfo[] library = RecordInfo.LoadDummyRecords(100);
        
        savedSelectedRecords = new int[boxes.Length];
        records = new Record[MAX_RECORDS_PER_BOX * boxes.Length];
        LoadRecordLibrary(library);
        Unselect();
    }
	
	void Update () {
        if (Input.GetAxis("Vertical") == 0 || (Input.GetAxis("Vertical") != 0 && Time.fixedTime - scrollTime > 0.5f)) {
            canScrollRecords = true;
        }
        if (Input.GetAxis("Horizontal") == 0 || (Input.GetAxis("Horizontal") != 0 && Time.fixedTime - scrollTime > 0.5f)) {
            canScrollBoxes = true;
        }
        int scrollAmount = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10 - Input.GetAxis("Vertical"));
        if (scrollAmount != 0 && canScrollRecords) {
            canScrollRecords = false;
            scrollTime = Time.fixedTime;
            ScrollRecords(scrollAmount > 0);
        }
        float boxScrollAmount = Input.GetAxis("Horizontal");
        if (boxScrollAmount != 0 && canScrollBoxes) {
            canScrollBoxes = false;
            scrollTime = Time.fixedTime;
            savedSelectedRecords[currentlySelectedBox] = currentlySelectedRecord;
            ScrollBoxes(boxScrollAmount < 0);
            currentlySelectedRecord = savedSelectedRecords[currentlySelectedBox];
        }

        if (Input.GetAxis("Action (Primary)") != 0) {
            Select();
        }
        if (Input.GetAxis("Action (Secondary)") != 0) {
            Unselect();
        }
    }

    void Select () {
        Record selectedRecord = GetRecordAt(currentlySelectedRecord);
        if (selectedRecord != null) {
            SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 100, 0);
            selectedRecord.SetSelected(true);
        }
    }

    void Unselect () {
        Record selectedRecord = GetRecordAt(currentlySelectedRecord);
        if (selectedRecord != null) {
            SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 20, 35);
            selectedRecord.SetSelected(false);
        }
    }

    void ScrollRecords(bool backwards = false, int iterations = 0) {
        int previouslySelected = currentlySelectedRecord;
        int boxSize = Mathf.Min(MAX_RECORDS_PER_BOX, amountOfRecords);
        currentlySelectedRecord = Mathf.Clamp(currentlySelectedRecord + (backwards ? -1 : 1), 0, boxSize - 1);
        if (previouslySelected != currentlySelectedRecord) {
            int direction = previouslySelected > currentlySelectedRecord ? 1 : -1;
            Record previous = GetRecordAt(previouslySelected);
            if (previous != null) {
                SetSpringValue(previous.GetComponent<HingeJoint>(), 20, 35 * direction);
                previous.SetSelected(false);
            }
            
            Unselect();
        }

        // Do the whole thing again if there are iterations and space
        if (currentlySelectedRecord > 0 && currentlySelectedRecord < boxSize - 1 && iterations > 0) {
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
        return records[(Mathf.Clamp(index, 0, MAX_RECORDS_PER_BOX - 1) + currentlySelectedBox * MAX_RECORDS_PER_BOX) % (MAX_RECORDS_PER_BOX * boxes.Length)];
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

            float recordOffset = (1.0f - (float)(amountOfRecords % MAX_RECORDS_PER_BOX) / MAX_RECORDS_PER_BOX) * 0.75f - 0.45f;
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
