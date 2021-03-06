﻿using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public static Dictionary<BeanType, bool> unlockedBeans = new Dictionary<BeanType, bool>()
    {
        { BeanType.BT_BASIC, true },
        { BeanType.BT_KILIMANJARO, false },
        { BeanType.BT_BLUE_MOUNTAIN, false },
        { BeanType.BT_MOCHA, false },
        { BeanType.BT_HOUSE_BLEND, false },
        { BeanType.BT_HOT_COCOA, false }
    };

    public static Dictionary<MilkType, bool> unlockedMilk = new Dictionary<MilkType, bool>()
    {
        { MilkType.Milk_NONE, true },
        { MilkType.Milk_SKIM, true },
        { MilkType.Milk_WHOLE, false },
        { MilkType.Milk_CREAM, false }
    };

    public static Dictionary<FlavorShot, bool> unlockedFlavors = new Dictionary<FlavorShot, bool>()
    {
        { FlavorShot.Flavor_NONE, true },
        { FlavorShot.Flavor_VANILLA, false },
        { FlavorShot.Flavor_MOCHA, false },
        { FlavorShot.Flavor_PUMPKIN_SPICE, false }
    };

    public static Dictionary<ToppingType, bool> unlockedToppings = new Dictionary<ToppingType, bool>()
    {
        //{ ToppingType.Topping_NONE, true },
        { ToppingType.Topping_WHIPPED_CREAM, false },
        { ToppingType.Topping_SPRINKLES, false },
        //{ ToppingType.Topping_COCOA_POWDER, false },
        { ToppingType.Topping_CINNAMON, false },
        { ToppingType.Topping_MARSHMALLOW, false }
    };

    Dictionary<BeanType, float> beanPrice = new Dictionary<BeanType, float>()
    {
        { BeanType.BT_BASIC, 2.0f },
        { BeanType.BT_KILIMANJARO, 2.35f },
        { BeanType.BT_BLUE_MOUNTAIN, 2.5f },
        { BeanType.BT_MOCHA, 2.75f },
        { BeanType.BT_HOUSE_BLEND, 5.50f },
        { BeanType.BT_HOT_COCOA, 3.0f }
    };

    [SerializeField]
    private GameObject[] beanObjects;
    [SerializeField]
    private GameObject[] milkObjects;
    [SerializeField]
    private GameObject[] flavorObjects;
    [SerializeField]
    private GameObject[] toppingObjects;

    private string filename = "/saveBeans.dat";
    public static float currentMoney = 50.0f;
    [SerializeField]
    private Text moneyText;
    private float moneyEarned = 0.0f;
    [SerializeField]
    private Text moneyEarnedText;
    [SerializeField]
    private GameObject[] reactionParticles = new GameObject[3];
    public GameObject[] ReactionParticles
    {
        get
        {
            return reactionParticles;
        }
    }
    [SerializeField]
    private Text shopNameText;
    [SerializeField]
    private Text justUnlockedText;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        /*List<int> test = new List<int>();
        test.Add(0);
        test.Add(5);
        test.Add(7);

        for (int i = 0; i < test.Count; i++)
        {
            Debug.Log(test[i]);
            test.Remove(test[i]);
            i--;
        }*/

        /*List<ToppingType> one = new List<ToppingType>();
        List<ToppingType> two = new List<ToppingType>();
        one.Add(ToppingType.Topping_WHIPPED_CREAM);
        one.Add(ToppingType.Topping_SPRINKLES);
        two.Add(ToppingType.Topping_WHIPPED_CREAM);
        two.Add(ToppingType.Topping_MARSHMALLOW);
        two.Add(ToppingType.Topping_COCOA_POWDER);
        Debug.Log(CompareToppingList(one, two));*/
        EnableAndDisableUnlockedItems();
        if (shopNameText != null && PlayerInfo.Instance != null)
        {
            shopNameText.text = "@" + PlayerInfo.Instance.shopName;
        }
	}
	
	// Update is called once per frame
    void Update()
    {
		if (moneyText != null)
        {
            moneyText.text = "$" + currentMoney.ToString("#.##");
        }
        if (moneyEarnedText != null)
        {
            moneyEarnedText.text = "Earned: $" + moneyEarned.ToString(".##");
        }
        //if (justUnlockedText != null)
        //{
            //justUnlockedText.text = "";
        //}
	}

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.OpenWrite(Application.persistentDataPath + filename);

        bf.Serialize(file, unlockedBeans);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + filename);

            unlockedBeans = (Dictionary<BeanType, bool>)bf.Deserialize(file);
            file.Close();
        }
    }

    public float GetCurrentMoney()
    {
        return currentMoney;
    }

    public void AlterCurrentMoney(float f)
    {
        //string formatted = f.ToString("#.##");
        currentMoney += f;
        moneyEarned += f;
    }

    public void ResetMoneyEarned()
    {
        moneyEarned = 0.0f;
    }

    /**
     * @brief Compares the items in two lists of toppings
     * @param coffeeToppings The list of toppings in the player-made coffee
     * @param orderToppings The list of toppings in the order
     * @returns The number of inconsistencies between the two lists
     */
    public int CompareToppingList(List<ToppingType> coffeeToppings, List<ToppingType> orderToppings)
    {
        int numMistakes = 0;

        for (int i = 0; i < coffeeToppings.Count; i++)
        {
            if (orderToppings.Contains(coffeeToppings[i]))
            {
                orderToppings.Remove(coffeeToppings[i]);
                coffeeToppings.Remove(coffeeToppings[i]);
                i--;
            }
        }

        numMistakes = coffeeToppings.Count;
        if (orderToppings.Count > numMistakes)
        {
            numMistakes = orderToppings.Count;
        }

        return numMistakes;
    }

    /**
     * @brief Compares the attributes of two Coffee objects
     * @param coffee The first Coffee object, which is the one the player made
     * @param order The second Coffee object, which is what the customer requested
     * @returns The number of inconsistencies between the two objects
     */
    public int CompareCoffee(Coffee coffee, Coffee order)
    {
        int numMistakes = 0;

        if (coffee.beanType != order.beanType)
        {
            numMistakes++;
        }
        if (coffee.milkType != order.milkType)
        {
            numMistakes++;
        }
        if (coffee.sugarAmount != order.sugarAmount)
        {
            numMistakes++;
        }
        if (coffee.flavor != order.flavor)
        {
            numMistakes++;
        }
        coffee.toppings.Sort();
        order.toppings.Sort();
        numMistakes += CompareToppingList(coffee.toppings, order.toppings);

        return numMistakes;
    }

    public float DetermineCoffeeAmount(Coffee c, float totalMulitpler = 1.0f, float beanMultiplier = 1.0f)
    {
        //float price = 0.0f;

        //price = beanPrice[c.beanType] * beanMultiplier;
        //price *= totalMulitpler;

        return beanPrice[c.beanType] * beanMultiplier * totalMulitpler;
    }

    /**
     * @brief Unlocks a certain item
     * @param itemType The type of item to unlock
     * @param item The index of the item that will be unlocked, must cast enum to int
     */
    public void UnlockItem(ItemType itemType, int item)
    {
        switch (itemType)
        {
            case ItemType.Item_BEAN:
                BeanType beanType = (BeanType)item;
                if (unlockedBeans.ContainsKey(beanType))
                {
                    unlockedBeans[beanType] = true;
                    justUnlockedText.text = "Unlocked: " + BeanTypeToString(beanType);
                }
                break;
            case ItemType.Item_MILK:
                MilkType milkType = (MilkType)item;
                if (unlockedMilk.ContainsKey(milkType))
                {
                    unlockedMilk[milkType] = true;
                    justUnlockedText.text = "Unlocked: " + MilkTypeToString(milkType);
                }
                break;
            case ItemType.Item_FLAVOR_SHOT:
                FlavorShot f = (FlavorShot)item;
                if (unlockedFlavors.ContainsKey(f))
                {
                    unlockedFlavors[f] = true;
                    justUnlockedText.text = "Unlocked: " + FlavorTypeToString(f);
                }
                break;
            case ItemType.Item_TOPPING:
                ToppingType t = (ToppingType)item;
                if (unlockedToppings.ContainsKey(t))
                {
                    unlockedToppings[t] = true;
                    justUnlockedText.text = "Unlocked: " + ToppingTypeToString(t);
                }
                break;
            default:
                break;
        }
    }

    public void UnlockNextBean(float price)
    {
        if (currentMoney >= price)
        {
            foreach (BeanType bt in unlockedBeans.Keys)
            {
                if (unlockedBeans[bt])
                {
                    continue;
                }
                else
                {
                    UnlockItem(ItemType.Item_BEAN, (int)bt);
                    currentMoney -= price;
                    break;
                }
            }
        }
    }

    public void UnlockNextMilk(float price)
    {
        if (currentMoney >= price)
        {
            foreach (MilkType mt in unlockedMilk.Keys)
            {
                if (!unlockedMilk[mt])
                {
                    UnlockItem(ItemType.Item_MILK, (int)mt);
                    currentMoney -= price;
                    break;
                }
            }
        }
    }

    public void UnlockNextFlavor(float price)
    {
        if (currentMoney >= price)
        {
            foreach (FlavorShot fs in unlockedFlavors.Keys)
            {
                if (!unlockedFlavors[fs])
                {
                    UnlockItem(ItemType.Item_FLAVOR_SHOT, (int)fs);
                    currentMoney -= price;
                    break;
                }
            }
        }
    }

    public void UnlockNextTopping(float price)
    {
        if (currentMoney >= price)
        {
            foreach (ToppingType tt in unlockedToppings.Keys)
            {
                if (!unlockedToppings[tt])
                {
                    UnlockItem(ItemType.Item_TOPPING, (int)tt);
                    currentMoney -= price;
                    break;
                }
            }
        }
    }

    /**
     * @brief Creates an order that the player must fulfill
     * @param numToppings (Optional) The number of toppings to decide on, max is 2, default is 0
     * @returns A Coffee object that holds the required information concerning the order
     */
    public Coffee GenerateOrder(int numToppings)
    {
        Coffee order = new Coffee();
        int index = 0;
        if (numToppings > 2)
        {
            numToppings = 2;
        }

        //decide on type of beans
        List<BeanType> availableBeans = new List<BeanType>();
        foreach (BeanType bean in unlockedBeans.Keys)
        {
            if (unlockedBeans[bean])
            {
                availableBeans.Add(bean);
            }
        }
        index = Random.Range(0, availableBeans.Count);
        order.SetBeanType(availableBeans[index]);

        //decide on type of milk
        List<MilkType> availableMilks = new List<MilkType>();
        foreach (MilkType milk in unlockedMilk.Keys)
        {
            if (unlockedMilk[milk])
            {
                availableMilks.Add(milk);
            }
        }
        index = Random.Range(0, availableMilks.Count);
        order.SetMilkType(availableMilks[index]);

        //decide on type of flavor
        List<FlavorShot> availableFlavors = new List<FlavorShot>();
        foreach (FlavorShot flavor in unlockedFlavors.Keys)
        {
            if (unlockedFlavors[flavor])
            {
                availableFlavors.Add(flavor);
            }
        }
        index = Random.Range(0, availableFlavors.Count);
        order.SetFlavorShot(availableFlavors[index]);

        //decide on number of toppings
        if (numToppings > 0)
        {
            List<ToppingType> availableToppings = new List<ToppingType>();
            foreach (ToppingType top in unlockedToppings.Keys)
            {
                if (unlockedToppings[top])
                {
                    availableToppings.Add(top);
                }
            }
            for (int i = 0; i < numToppings; i++)
            {
                if (availableToppings.Count == 0)
                {
                    break;
                }
                index = Random.Range(0, availableToppings.Count);
                order.AddTopping(availableToppings[index]);
                availableToppings.Remove(availableToppings[index]);
            }
        }

        //decide amount of sugar
        order.SetSugarAmount(Random.Range(0, 3));

        return order;
    }

    public string BeanTypeToString(BeanType bt)
    {
        switch (bt)
        {
            case BeanType.BT_BASIC:
                return "Basic Blend (B)";
            case BeanType.BT_BLUE_MOUNTAIN:
                return "Blue Mountain (Triangle Shape)";
            case BeanType.BT_HOT_COCOA:
                return "Hot Cocoa (Chocolate Bar)";
            case BeanType.BT_HOUSE_BLEND:
                return "House Blend (Sparkles)";
            case BeanType.BT_KILIMANJARO:
                return "Kilimanjaro (K)";
            case BeanType.BT_MOCHA:
                return "Mocha (M)";
            default:
                return "idk";
        }
    }

    public string MilkTypeToString(MilkType mt)
    {
        switch (mt)
        {
            case MilkType.Milk_CREAM:
                return "Cream (C)";
            case MilkType.Milk_SKIM:
                return "Skim (S)";
            case MilkType.Milk_WHOLE:
                return "Whole (W)";
            default:
                return "idk";
        }
    }

    public string FlavorTypeToString(FlavorShot fs)
    {
        switch (fs)
        {
            case FlavorShot.Flavor_MOCHA:
                return "Mocha (M)";
            case FlavorShot.Flavor_PUMPKIN_SPICE:
                return "Pumpkin Spice (Ps)";
            case FlavorShot.Flavor_VANILLA:
                return "Vanilla (V)";
            default:
                return "idk";
        }
    }

    public string ToppingTypeToString(ToppingType tt)
    {
        string s = "";

        switch (tt)
        {
            case ToppingType.Topping_CINNAMON:
                s = "cinnamon";
                break;
            //case ToppingType.Topping_COCOA_POWDER:
                //s = "cocoa powder";
                //break;
            case ToppingType.Topping_MARSHMALLOW:
                s = "marshmallow";
                break;
            case ToppingType.Topping_SPRINKLES:
                s = "sprinkles";
                break;
            case ToppingType.Topping_WHIPPED_CREAM:
                s = "whipped cream";
                break;
            case ToppingType.Topping_NONE:
                s = "no topping";
                break;
            default:
                s = "unknown";
                break;
        }

        return s;
    }

    public void EnableAndDisableUnlockedItems()
    {
        for (int i = 0; i < 6; i++)
        {
            if (unlockedBeans[(BeanType)i])
            {
                if (beanObjects[i] != null)
                {
                    beanObjects[i].SetActive(true);
                }
            }
            else
            {
                if (beanObjects[i] != null)
                {
                    beanObjects[i].SetActive(false);
                }
            }
        }

        for (int i = 1; i < 4; i++)
        {
            if (unlockedMilk[(MilkType)i])
            {
                if (milkObjects[i] != null)
                {
                    milkObjects[i].SetActive(true);
                }
            }
            else
            {
                if (milkObjects[i] != null)
                {
                    milkObjects[i].SetActive(false);
                }
            }
        }

        for (int i = 1; i < 4; i++)
        {
            if (unlockedFlavors[(FlavorShot)i])
            {
                if (flavorObjects[i] != null)
                {
                    flavorObjects[i].SetActive(true);
                }
            }
            else
            {
                if (flavorObjects[i] != null)
                {
                    flavorObjects[i].SetActive(false);
                }
            }
        }

        for (int i = 1; i < 5; i++)
        {
            if (unlockedToppings[(ToppingType)i])
            {
                if (toppingObjects[i] != null)
                {
                    toppingObjects[i].SetActive(true);
                }
            }
            else
            {
                if (toppingObjects[i] != null)
                {
                    toppingObjects[i].SetActive(false);
                }
            }
        }
    }

    public void StartLevel()
    {
        moneyEarned = 0.0f;
        EnableAndDisableUnlockedItems();
    }
}
