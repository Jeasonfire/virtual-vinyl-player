using UnityEngine;
using System.Net;
using System.Threading;
using System.Collections;

public class RecordManager : MonoBehaviour {
    public const int MAX_RECORDS_PER_BOX = 8;

    public GameObject recordTemplate;
    public GameObject[] boxes;
    public RecordPlayer recordPlayer;
    public CameraManager cam;
    public Vector3 forward;

    private RecordCase[] records;
    private int amountOfRecords;
    private int currentlySelectedRecord = 0;
    private int[] savedSelectedRecords;
    private int currentlySelectedBox = 0;

    private bool interacting = true;
    private float interactingStartedAt = 0;

    private float scrollTime = 0;
    private bool selected = false;
    private bool canScrollRecords = true;
    private bool canScrollBoxes = true;
    private bool canSelect = true;
    private bool canSpin = true;

    void Start() {
        savedSelectedRecords = new int[boxes.Length];
        records = new RecordCase[MAX_RECORDS_PER_BOX * boxes.Length];
        StartCoroutine("LoadRecordLibrary");
    }
	
	void Update () {
        if (!interacting) {
            return;
        }

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
            ScrollBoxes(boxScrollAmount < 0);
        }

        if (!Input.GetButton("Action (Primary)")) {
            canSelect = true;
        }
        if (Input.GetButton("Action (Primary)") && canSelect && !recordPlayer.loadingSongs) {
            bool shouldSelect = true;
            GameObject hovered = Util.GetHoveredGameObject();
            if (hovered != null) {
                BoxID boxID = hovered.GetComponent<BoxID>();
                if (boxID != null) {
                    int boxIndex = boxID.id;
                    if (currentlySelectedBox != boxIndex) {
                        ScrollBoxes(currentlySelectedBox > boxIndex, Mathf.Abs(boxIndex - currentlySelectedBox));
                        shouldSelect = false;
                    }
                }
                PlayButton play = hovered.GetComponent<PlayButton>();
                if (play != null) {
                    ChooseSong();
                    shouldSelect = false;
                }
            }
            
            if (shouldSelect) {
                if (selected) {
                    Unselect();
                } else {
                    Select();
                }
            }
            canSelect = false;
        }
        
        if (!Input.GetButton("Action (Secondary)")) {
            canSpin = true;
        }
        if (Input.GetButton("Action (Secondary)") && canSpin && selected) {
            RecordCase selectedRecord = GetRecordAt(currentlySelectedRecord);
            if (selectedRecord != null) {
                selectedRecord.Flip();
            }
            canSpin = false;
        }

        if (Input.GetButton("Action (Tertiary)") && !recordPlayer.loadingSongs && Time.time - interactingStartedAt > 0.3) {
            ChangeToRecordPlayer();
        }
    }

    void ChangeToRecordPlayer () {
        interacting = false;
        Unselect();
        recordPlayer.StartInteracting();
    }

    public void StartInteracting() {
        interacting = true;
        interactingStartedAt = Time.time;
        cam.targetPosition = cam.GetDefaultPosition();
        cam.targetPosition.z = transform.position.z + currentlySelectedBox * 1.5f;
        cam.targetRotation = cam.GetDefaultRotation();
        cam.targetRotation.y = 270;
        cam.targetFov = cam.GetDefaultFov();
    }

    void ChooseSong () {
        RecordCase selectedRecord = GetRecordAt(currentlySelectedRecord);
        if (selectedRecord != null) {
            recordPlayer.StartLoadingSong(selectedRecord);
        }
    }

    void Select () {
        RecordCase selectedRecord = GetRecordAt(currentlySelectedRecord);
        if (selectedRecord != null) {
            selected = true;
            SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 500, 0);
            selectedRecord.SetSelected(true);
            cam.targetRotation.x = 0;
            float deltaFov = 16;
            cam.targetFov = cam.GetDefaultFov() - deltaFov * (1f - (float)currentlySelectedRecord / MAX_RECORDS_PER_BOX);
        }
    }

    void Unselect () {
        RecordCase selectedRecord = GetRecordAt(currentlySelectedRecord);
        if (selectedRecord != null) {
            if (selected) {
                SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 30, 50);
            } else {
                SetSpringValue(selectedRecord.GetComponent<HingeJoint>(), 20, 50);
            }
            selected = false;
            selectedRecord.SetSelected(false);
            cam.targetRotation.x = cam.GetDefaultRotation().x;
            cam.targetFov = cam.GetDefaultFov();
        }
    }

    void ScrollRecords(bool backwards = false, int iterations = 1) {
        if (iterations < 1) {
            return;
        }

        int previouslySelected = currentlySelectedRecord;
        int boxSize = RecordsInBox(currentlySelectedBox);
        currentlySelectedRecord = Mathf.Clamp(currentlySelectedRecord + (backwards ? 1 : -1), 0, boxSize - 1);
        if (previouslySelected != currentlySelectedRecord) {
            int direction = previouslySelected > currentlySelectedRecord ? -1 : 1;
            RecordCase previous = GetRecordAt(previouslySelected);
            if (previous != null) {
                SetSpringValue(previous.GetComponent<HingeJoint>(), 20, 50 * direction);
                previous.SetSelected(false);
                previous.PlayWooshSound();
            }
            
            Unselect();
        }

        // Do the whole thing again if there are iterations and space
        if (currentlySelectedRecord > 0 && currentlySelectedRecord < boxSize - 1 && iterations > 1) {
            ScrollRecords(backwards, iterations - 1);
        }
    }

    void ScrollBoxes (bool backwards = false, int iterations = 1) {
        if (iterations < 1) {
            return;
        }

        savedSelectedRecords[currentlySelectedBox] = currentlySelectedRecord;

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

        currentlySelectedRecord = savedSelectedRecords[currentlySelectedBox];

        // Do the whole thing again if there are iterations and space
        if (currentlySelectedBox > 0 && currentlySelectedBox < boxes.Length - 1 && iterations > 1) {
            ScrollBoxes(backwards, iterations - 1);
        }
    }

    void SetSpringValue(HingeJoint joint, float spring, float targetPosition) {
        JointSpring tempSpring = joint.spring;
        tempSpring.spring = spring;
        tempSpring.targetPosition = targetPosition;
        joint.spring = tempSpring;
    }

    RecordCase GetRecordAt (int index) {
        return GetRecordAt (index, currentlySelectedBox);
    }

    RecordCase GetRecordAt (int index, int boxIndex) {
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

    IEnumerator LoadRecordLibrary () {
        AlbumLoader loader = new AlbumLoader(MAX_RECORDS_PER_BOX * boxes.Length);
        loader.Load();
        while (!loader.IsDone()) {
            Album info;
            if ((info = loader.PopLatestRecordInfo()) != null) {
                CreateRecord(info);
            }
            yield return null;
        }
    }

    private void CreateRecord (Album album) {
        if (amountOfRecords >= MAX_RECORDS_PER_BOX * boxes.Length) {
            Debug.LogError("Tried to add record '" + album.artist + " - " + album.name + "' to a full box!");
        } else {
            int boxIndex = amountOfRecords / MAX_RECORDS_PER_BOX;
            int recordIndex = amountOfRecords % MAX_RECORDS_PER_BOX;
            Transform box = boxes[boxIndex].transform;
            savedSelectedRecords[boxIndex] = recordIndex;
            if (currentlySelectedBox == boxIndex) {
                Unselect();
                currentlySelectedRecord = recordIndex;
            }

            float recordOffset = ((float)recordIndex / MAX_RECORDS_PER_BOX) * 0.8f - 0.4f;
            Vector3 positionOffset = new Vector3(recordOffset, 0, 0);

            GameObject newRecordObject = (GameObject)Instantiate(recordTemplate, box.position + positionOffset, recordTemplate.transform.rotation);
            newRecordObject.transform.parent = box;
            newRecordObject.name = "Record #" + amountOfRecords;

            RecordCase newRecord = newRecordObject.GetComponent<RecordCase>();
            newRecord.album = album;
            records[amountOfRecords++] = newRecord;
        }
    }
}
