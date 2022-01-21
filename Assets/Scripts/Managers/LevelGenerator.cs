using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    /// <summary>
    /// This script generates the levels based on build index.
    /// All instantiated objects are created in spawn area.
    /// </summary>
    
    public static LevelGenerator Instance;
    private GameEngine _gameEngine;
    private Vector3 _spawnAreaScale;
    private int _currentLevel;
    private List<int> _matrixNumbers;
    private List<Vector3> _spawnPositions;
    [Tooltip("Prefabs are listed in level order")]
    [SerializeField] private List<GameObject> levelFruitPrefabs;
    [SerializeField] private Transform spawnAreTr;
    [SerializeField] private int spawnCount;
    [Tooltip("For Level 3 to decide matrix")]
    [SerializeField] private bool readFromTexture;
    
    public int GetSpawnCount => spawnCount;
    public List<GameObject> GetLevelFruitPrefabs => levelFruitPrefabs;
    public Vector3 GetSpawnAreaScale => _spawnAreaScale;
    private class Matrix
    {
        public List<int> matrix = new List<int>();
    }
    private void Awake()
    {
        Instance = this;
        _currentLevel = LevelManager.GetLevelIndex();
    }
    private void Start()
    {
        _gameEngine=GameEngine.Instance;
        _spawnAreaScale = spawnAreTr.localScale;
        SetLevel();
    }
    public void SetLevel()
    {
        if(_currentLevel==1) SetLevel1();
        if(_currentLevel==2) SetLevel2();
        if(_currentLevel==3) SetLevel3();
        if(_currentLevel==4) SetLevel4();
    }
    private void SetLevel1()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var randomX = Random.Range(-_spawnAreaScale.x/2f, _spawnAreaScale.x/2f);
            var randomZ = Random.Range(-_spawnAreaScale.z/2f, _spawnAreaScale.z/2f);
            var spawnPos = new Vector3(randomX, 0, randomZ);
            var olive=Instantiate(levelFruitPrefabs[0], spawnPos, Quaternion.identity);
            olive.AddComponent<Collectable>();
        }
    }
    private void SetLevel2()
    {
        _spawnPositions = new List<Vector3>();
        for (float i = -_spawnAreaScale.x/2f; i < (_spawnAreaScale.x/2)+0.01f; i+=0.5f)
        {
            for (float j = -_spawnAreaScale.z/2f; j < (_spawnAreaScale.z/2)+0.01f; j+=0.5f)
            {
                _spawnPositions.Add(new Vector3(i,0,j));
            }
        }
        for (int i = 0; i < spawnCount; i++)
        {
            var spawnPos = _spawnPositions[Random.Range(0, _spawnPositions.Count)];
            var olive=Instantiate(levelFruitPrefabs[0],spawnPos , Quaternion.identity);
            _spawnPositions.Remove(spawnPos);
            olive.AddComponent<Mergeable>();
        }
    }

    private void SetLevel3()
    {
        var s = "MatrixDefault";
        if (readFromTexture) s = "MatrixTexture";
        var jsonText = Resources.Load<TextAsset>(s).text;
        _matrixNumbers= JsonUtility.FromJson<Matrix>(jsonText).matrix;
        var iCount = 0;
        for (float i = _spawnAreaScale.z/2f; i > -(_spawnAreaScale.z/2)-0.01f; i--)
        {
            var jCount = 0;
            for (float j = -_spawnAreaScale.x/2f; j < (_spawnAreaScale.x/2)+0.01f; j++)
            {
                var gridNumber = (iCount * 5) + jCount;
                var number = _matrixNumbers[gridNumber];
                var spawnPos = new Vector3(j, 0, i);
                var fruit=Instantiate(levelFruitPrefabs[number], spawnPos, Quaternion.identity);
                var sM = fruit.AddComponent<SlideMergeable>();
                sM.GridDetail = (gridNumber,number);
                _gameEngine.GridFruitDictionary.Add(gridNumber,number);
                _gameEngine.AddToGridSmDictionary(gridNumber,sM);
                _gameEngine.GridPositionDictionary.Add(gridNumber,spawnPos);
                jCount++;
            }
            iCount++;
        }
    }
    private void SetLevel4()
    {
        _spawnPositions = new List<Vector3>();
        for (float i = -_spawnAreaScale.x/2f; i < (_spawnAreaScale.x/2)+0.01f; i++)
        {
            for (float j = -_spawnAreaScale.z/2f; j < (_spawnAreaScale.z/2)+0.01f; j++)
            {
                _spawnPositions.Add(new Vector3(i,0,j));
            }
        }
        for (int i = 0; i < spawnCount; i++)
        {
            var spawnPos = _spawnPositions[Random.Range(0, _spawnPositions.Count)];
            var random = Random.Range(0, levelFruitPrefabs.Count);
            var fruit=Instantiate(levelFruitPrefabs[random],spawnPos , Quaternion.identity);
            _spawnPositions.Remove(spawnPos);
            var brick = fruit.AddComponent<Brick>();
            brick.BrickType = random;
        }
    }
}
