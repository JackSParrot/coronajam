using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public static Stage Instance;

    [SerializeField]
    Transform _carsParent = null;
    [SerializeField]
    Transform _objectivesParent = null;
    [SerializeField]
    TMPro.TextMeshProUGUI _citizens = null;
    [SerializeField]
    TMPro.TextMeshProUGUI _enemies = null;
    [SerializeField]
    TMPro.TextMeshProUGUI _gameOverText = null;
    [SerializeField]
    GameObject _gameOverPanel = null;

    [HideInInspector]
    public List<Transform> Objectives = new List<Transform>();

    [HideInInspector]
    public List<Transform> Cars = new List<Transform>();
    [HideInInspector]
    public List<Enemy> Enemies = new List<Enemy>();
    [HideInInspector]
    public List<Citizen> Citizens = new List<Citizen>();


    [HideInInspector]
    public int Score = 0;

    public void RescueCitizen()
    {
        Score++;
    }

    public void AddCitizen(Citizen c)
    {
        Citizens.Add(c);
        _citizens.text = Citizens.Count.ToString();
    }
    public void RemoveCitizen(Citizen c)
    {
        Citizens.Remove(c);
        _citizens.text = Citizens.Count.ToString();
        if (Citizens.Count < 1)
        {
            _gameOverPanel.SetActive(true);
            _gameOverText.text = Score.ToString();
        }
    }

    public void AddZombie(Enemy z)
    {
        Enemies.Add(z);
        _enemies.text = Enemies.Count.ToString();
    }

    public void RemoveZombie(Enemy z)
    {
        Enemies.Remove(z);
        _enemies.text = Enemies.Count.ToString();
        if(Enemies.Count < 1)
        {
            _gameOverPanel.SetActive(true);
            _gameOverText.text = (Citizens.Count + Score).ToString();
        }
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void Awake()
    {
        Instance = this;
        _gameOverPanel.SetActive(false);
        Enemies.Clear();
        Citizens.Clear();
        Score = 0;

        Cars.Clear();
        for (int i = 0; i < _carsParent.childCount; ++i)
        {
            Cars.Add(_carsParent.GetChild(i));
        }
        Objectives.Clear();
        for (int i = 0; i < _objectivesParent.childCount; ++i)
        {
            Objectives.Add(_objectivesParent.GetChild(i));
        }
    }
}
