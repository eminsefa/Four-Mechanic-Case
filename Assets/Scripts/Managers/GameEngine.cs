using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    /// <summary>
    /// This script controls the game play.
    /// Updates the ui manager.
    /// </summary>
    
    public static GameEngine Instance;
    private UIManager _uiManager;
    private LevelGenerator _levelGenerator;
    private int _spawnCount;
    private int _destroyedFoodCount;
    private List<GameObject> _levelFruitPrefabs;
    [SerializeField] private List<GameObject> allFruitPrefabs;
    
    //---Level 3---
    private bool _fruitsSliding;
    private Vector3 _spawnAreaScale;
    public Dictionary<int, int> GridFruitDictionary { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, SlideMergeable> GridSmDictionary { get; set; } = new Dictionary<int, SlideMergeable>();
    public Dictionary<int, Vector3> GridPositionDictionary { get; set; } = new Dictionary<int, Vector3>();
    private List<SlideMergeable> _allSlideMergeableList = new List<SlideMergeable>();
    public void AddToGridSmDictionary(int number, SlideMergeable sm)
    {
        GridSmDictionary.Add(number,sm);
        _allSlideMergeableList.Add(sm);
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _uiManager=UIManager.Instance;
        _levelGenerator=LevelGenerator.Instance;
        _spawnCount = _levelGenerator.GetSpawnCount;
        _levelFruitPrefabs = _levelGenerator.GetLevelFruitPrefabs;
        _spawnAreaScale = _levelGenerator.GetSpawnAreaScale;
    }
    public void FruitDestroyed()
    {
        _destroyedFoodCount++;
        var percentage = Mathf.Lerp(0, 1, (float)_destroyedFoodCount / _spawnCount);
        _uiManager.SetFillImage(percentage);
        if(_destroyedFoodCount>=_spawnCount)
        {
            StopAllCoroutines();
            _uiManager.LevelCompleted();
            FruitGained();
        }
    }
    public void FruitsMerged(Vector3 middlePos,int mergedCount)
    {
        var newMergedCount = mergedCount + 1;
        var fruit = Instantiate(_levelFruitPrefabs[newMergedCount], middlePos, Quaternion.identity);
        var m=fruit.AddComponent<Mergeable>();
        m.MergedCount = newMergedCount;
        FruitDestroyed();
    }

    public IEnumerator CheckToMerge(SlideMergeable sm)
    {
        yield return new WaitForEndOfFrame();
        CheckIfFruitsMoving();
        while (_fruitsSliding)
        {
            yield return null;
        }
        var gridNumber = sm.GridDetail.gridNumber;
        var sameFruitList = new List<int>();

        #region CheckNeighbours

        var leftNumber = gridNumber - 1;
        if (leftNumber >= 0 && gridNumber / 5 == leftNumber / 5 && GridSmDictionary[leftNumber])
        {
            if (GridFruitDictionary[leftNumber] == GridFruitDictionary[gridNumber])
            {
                sameFruitList.Add(leftNumber);
                var leftMoreNumber = leftNumber - 1;
                if (leftMoreNumber >= 0 && gridNumber / 5 == leftMoreNumber / 5  && GridSmDictionary[leftMoreNumber])
                {
                    if (GridFruitDictionary[leftMoreNumber] == GridFruitDictionary[gridNumber])
                    {
                        sameFruitList.Add(leftMoreNumber);
                    }
                }
            }
        }

        var rightNumber = gridNumber + 1;
        if (rightNumber < 30 && gridNumber / 5 == rightNumber / 5 && GridSmDictionary[rightNumber])
        {
            if (GridFruitDictionary[rightNumber] == GridFruitDictionary[gridNumber])
            {
                sameFruitList.Add(rightNumber);
                var rightMoreNumber = rightNumber + 1;
                if (rightMoreNumber < 30 && gridNumber / 5 == rightMoreNumber / 5 && GridSmDictionary[rightMoreNumber])
                {
                    if (GridFruitDictionary[rightMoreNumber] == GridFruitDictionary[gridNumber])
                    {
                        sameFruitList.Add(rightMoreNumber);
                    }
                }
            }
        }

        if (sameFruitList.Count < 2) sameFruitList.Clear();
        
        var upNumber = gridNumber - 5;
        if (upNumber >=0 && GridSmDictionary[upNumber])
        {
            if (GridFruitDictionary[upNumber] == GridFruitDictionary[gridNumber])
            {
                sameFruitList.Add(upNumber);
                var upMoreNumber = upNumber -5;
                if (upMoreNumber >= 0  && GridSmDictionary[upMoreNumber])
                {
                    if (GridFruitDictionary[upMoreNumber] == GridFruitDictionary[gridNumber])
                    {
                        sameFruitList.Add(upMoreNumber);
                    }
                }
            }
        }
        
        var downNumber = gridNumber + 5;
        if (downNumber < 30 && GridSmDictionary[downNumber])
        {
            if (GridFruitDictionary[downNumber] == GridFruitDictionary[gridNumber])
            {
                sameFruitList.Add(downNumber);
                var downMoreNumber = downNumber + 5;
                if (downMoreNumber < 30 && GridSmDictionary[downMoreNumber])
                {
                    if (GridFruitDictionary[downMoreNumber] == GridFruitDictionary[gridNumber])
                    {
                        sameFruitList.Add(downMoreNumber);
                    }
                }
            }
        }

        #endregion
        
        if (sameFruitList.Count >= 2)
        {
            sameFruitList.Add(gridNumber);
            for (int i = 0; i < sameFruitList.Count; i++)
            {
                var number = sameFruitList[i];
                Destroy(GridSmDictionary[number].gameObject);
                GridSmDictionary[number] = null;
                FruitDestroyed();
            }
            StartCoroutine(FillGridEmptySpaces());
        }
        else
        {
            yield return new WaitForEndOfFrame();
            if (sm && !_fruitsSliding) sm.SlideBack();
        }
    }
    private IEnumerator FillGridEmptySpaces()
    {
        CheckIfFruitsMoving();
        while (_fruitsSliding)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        for (int i = _spawnCount-1; i >= 0; i--)
        {
            var sM = GridSmDictionary[i];
            if(!sM) continue;
            var nextNumber = i;
            for (int j = 1; j < _spawnAreaScale.z+1; j++)
            {
                var checkNumber = i + j * (int)(_spawnAreaScale.x+1);
                if(checkNumber>_spawnCount-1) continue;
                if (!GridSmDictionary[checkNumber]) nextNumber = checkNumber;
                else break;
            }
            if(nextNumber==i) continue;
            var pos = GridPositionDictionary[nextNumber];
            var gd = sM.GridDetail;
            var oldNumber = gd.gridNumber;
            gd.gridNumber = nextNumber;
            sM.GridDetail = gd;
            GridSmDictionary[nextNumber] = sM;
            GridFruitDictionary[nextNumber] = gd.fruitNumber;
            GridSmDictionary[oldNumber] = null;
            sM.FillSpace(pos);
            _fruitsSliding = true;
        }
    }
    private void CheckIfFruitsMoving()
    {
        //Could find a better way
        foreach (var sM in _allSlideMergeableList)
        {
            if (sM && sM.IsMoving)
            {
                _fruitsSliding = true;
                return;
            }
        }
        _fruitsSliding = false;
    }
    private void FruitGained()
    {
        var random = Random.Range(0, allFruitPrefabs.Count);
        var f = Instantiate(allFruitPrefabs[random]);
        var count=PlayerPrefs.GetInt(random.ToString(), 0);
        PlayerPrefs.SetInt(random.ToString(),count+1);
        f.AddComponent<FruitVisualiser>();
    }
}
