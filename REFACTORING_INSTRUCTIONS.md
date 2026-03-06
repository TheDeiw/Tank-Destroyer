# Інструкція з рефакторингу проекту MapzLabs

## Загальний план

Видаляємо патерни, які не потрібні для геймплею: **Decorator (куль)**, **State**, **Observer**, **Flyweight**, **Custom Attribute**.
Залишаємо те, що реально працює: **Abstract Factory**, **Builder**, **Strategy** (для ворогів).
Виправляємо баги та порушення SOLID.

---

## Крок 1. Видалити файли

Видалити повністю ці файли (та їх .meta):

| Файл                                              | Причина                                                 |
| ------------------------------------------------- | ------------------------------------------------------- |
| `Assets/Scripts/Enemy/--EnemyDestroy.cs`          | Старий код, замінений PlayerShooting                    |
| `Assets/Scripts/Enemy/--EnemySpawner.cs`          | Старий код, замінений GameManager                       |
| `Assets/Scripts/CheckDeath.cs`                    | Дублює перевірку смерті, прив'язаний до slider напряму  |
| `Assets/Scripts/Bullets/BulletEffectAttribute.cs` | Custom Attribute, не потрібен без декораторів           |
| `Assets/Scripts/PlayerState.cs`                   | State патерн, ніде не активується                       |
| `Assets/Scripts/PlayerStateManager.cs`            | State machine, без State не потрібен                    |
| `Assets/Scripts/Observer/Observer.cs`             | Observer інтерфейси та EventArgs, ніхто не підписується |
| `Assets/Scripts/Observer/Publisher.cs`            | Generic publisher, не використовується реально          |
| `Assets/Scripts/Observer/GameEventManager.cs`     | Singleton менеджер подій, не підключений                |
| `Assets/Tests/EditMode/DecoratorTest.cs`          | Тести для декораторів, які видаляємо                    |
| `tskt Test Task.sln`                              | Застарілий .sln файл                                    |

Після видалення Observer файлів видалити папку `Assets/Scripts/Observer/` та її `.meta`.

---

## Крок 2. Спростити систему куль (прибрати Decorator + Flyweight)

### 2.1. Переписати `BulletSystem.cs`

Видалити ВСЕ з файлу. Замінити на просту структуру без декораторів, без Flyweight-кешу, без IBullet-хієрархії:

```csharp
using UnityEngine;

public class BulletData
{
    public int Damage { get; }
    public float Speed { get; }
    public Vector3 Direction { get; private set; }

    public BulletData(int damage, float speed)
    {
        Damage = damage;
        Speed = speed;
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }
}
```

Тут `BulletData` — це простий клас даних, який зберігає параметри снаряда. Немає інтерфейсу, немає декораторів, немає фабрики з кешем. Якщо потрібні різні типи куль — додаєте різні конструктори або статичні методи.

### 2.2. Оновити `BulletMovement.cs`

Замінити `IBullet` на `BulletData`:

```csharp
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private BulletData bullet;

    public void SetBullet(BulletData bullet)
    {
        this.bullet = bullet;
    }

    public void SetDirection(Vector3 dir)
    {
        bullet.SetDirection(dir);
    }

    void Update()
    {
        transform.position += bullet.Direction * bullet.Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
```

### 2.3. Оновити `PlayerShooting.cs`

Рядок 38 — замінити створення кулі:

**Було:**

```csharp
IBullet bullet = BulletFactory.CreateStandardBullet();
```

**Стало:**

```csharp
BulletData bullet = new BulletData(1, 20f);
```

Видалити рядки 40-41 (закоментовані виклики `CreateEnhancedBullet` / `CreateFastBullet`).

Метод `SetBullet` тепер приймає `BulletData` замість `IBullet`.

### 2.4. Оновити `BulletSpawner.cs`

Видалити закоментований код (рядки 14-30). В методі `LaunchBullet` замінити:

**Було:**

```csharp
IBullet bullet = BulletFactory.CreateStandardBullet();
```

**Стало:**

```csharp
BulletData bullet = new BulletData(1, 20f);
```

Видалити рядки 41-43 (закоментовані альтернативи).

---

## Крок 3. Спростити PlayerHealth (прибрати Observer)

### 3.1. Переписати `PlayerHealth.cs`

Прибрати всі виклики `GameEventManager.Instance`. Здоров'я гравця має працювати напряму з HealthBar і простим callback для смерті.

```csharp
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    private int maxHealthValue;
    private int currentHealthValue;
    private bool isDead = false;

    public event Action OnPlayerDied;

    public void InitializeHealth(int maxHealth)
    {
        maxHealthValue = maxHealth;
        currentHealthValue = maxHealth;
        isDead = false;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealthValue -= damage;
        currentHealthValue = Mathf.Clamp(currentHealthValue, 0, maxHealthValue);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealthValue);
        }

        if (currentHealthValue <= 0)
        {
            isDead = true;
            OnPlayerDied?.Invoke();
        }
    }

    public bool IsAlive() => !isDead && currentHealthValue > 0;
}
```

Ключові зміни:

- Видалений `_healthBarReferenceForInitialSetup` (мертве поле) — замінений на робочий `healthBar`
- Видалені всі звернення до `GameEventManager`
- Замість Observer — простий C# `event Action OnPlayerDied`, яким може скористатися будь-який скрипт
- Метод `ApplyDamage` перейменований в `TakeDamage` (зрозуміліше)
- `healthBar.SetHealth()` викликається напряму

### 3.2. Замінити функціонал `CheckDeath.cs`

`CheckDeath.cs` видаляємо (крок 1). Але хтось має показувати lose panel. Цю логіку можна перенести в новий простий скрипт або в існуючий компонент на сцені:

Створити `Assets/Scripts/UI/GameOverHandler.cs`:

```csharp
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject losePanel;

    void Start()
    {
        Time.timeScale = 1f;
        losePanel.SetActive(false);
        playerHealth.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnPlayerDied -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
```

На сцені: повісити цей скрипт замість `CheckDeath`, призначити ті самі поля через Inspector.

---

## Крок 4. Прибрати State з PlayerMovement

### 4.1. Оновити `Player Movement.cs`

Видалити поля `baseSpeed` і `stateManager` (рядки 22-23). Видалити рядок 44 (обчислення `currentSpeed`, яке все одно ігнорувалось). Поле `speed` (рядок 10) вже і так використовується для руху.

**Видалити:**

```csharp
public float baseSpeed = 12f;
private PlayerStateManager stateManager;
```

**Видалити:**

```csharp
float currentSpeed = baseSpeed * (stateManager != null ? stateManager.GetCurrentSpeedMultiplier() : 1f);
```

Рядок 56 (`controller.Move(moveDirection.normalized * speed * Time.deltaTime)`) залишається — він і так працює правильно з полем `speed`.

---

## Крок 5. Виправити баг Singleton в GameManager

### 5.1. Виправити `GameManager.cs`, рядок 28

**Було:**

```csharp
void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        Instantiate(gameObject);  // <-- БАГ: створює клон!
    }
    else
    {
        Destroy(gameObject);
    }

    enemyFactories = new EnemyFactory[]  // <-- виконується навіть для клона
    {
        new EasyEnemyFactory(prefabProvider),
        new HardEnemyFactory(prefabProvider)
    };
}
```

**Стало:**

```csharp
void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        // DontDestroyOnLoad тільки якщо GameManager має жити між сценами.
        // Якщо ні — просто прибрати цей рядок.
        // DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
        return;  // ВАЖЛИВО: return щоб клон не ініціалізував фабрики
    }

    enemyFactories = new EnemyFactory[]
    {
        new EasyEnemyFactory(prefabProvider),
        new HardEnemyFactory(prefabProvider)
    };
}
```

Два виправлення:

1. **Замінити `Instantiate(gameObject)` на нічого** (або `DontDestroyOnLoad` якщо треба). `Instantiate` створював клон GameManager безглуздо.
2. **Додати `return` після `Destroy`** — інакше клон ще встигає ініціалізувати `enemyFactories` до знищення.

Також видалити закоментований рядок 9 (`//public GameObject[] enemyPrefabs;`).

---

## Крок 6. Виправити порушення SOLID

### 6.1. SRP: Виділити звук з HealthBar

`HealthBar.cs` рядок 15 — `explosionSound.Play()` всередині `SetHealth()`. Health bar не повинен відповідати за звуки.

**Рішення:** Прибрати звук з HealthBar. Перенести відтворення звуку в `BulletDetection.cs` (де відбувається попадання):

В `HealthBar.cs` — видалити поле `explosionSound` і виклик `explosionSound.Play()`:

```csharp
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }
}
```

В `BulletDetection.cs` — додати `[SerializeField] private AudioSource hitSound;` і грати його в `OnTriggerEnter` при попаданні.

### 6.2. SRP: BulletDetection — додати прапорець isDead

`BulletDetection.cs` — перевірка смерті в `Update()` може спрацювати повторно до того як `Destroy` виконається.

**Рішення:** Додати `private bool isDead = false;` і перевіряти його:

```csharp
void Update()
{
    if (!isDead && currentHealth <= 0)
    {
        isDead = true;
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
```

Також прибрати закоментований код в `OnTriggerEnter` (рядки 44-48).

### 6.3. OCP: GameManager — не хардкодити фабрики

`GameManager.cs` — масив фабрик захардкоджений в `Awake()`. Для додавання нового типу ворога треба правити GameManager.

**Рішення:** Зробити фабрики настроюваними через SerializeField або винести створення масиву фабрик назовні. Мінімальне виправлення:

```csharp
// Замість хардкоду фабрик у Awake, дозволити конфігурувати їх:
[SerializeField] private PrefabProvider prefabProvider;

// Фабрики створюються на основі PrefabProvider, але масив не захардкоджений.
// Як варіант — зробити їх [SerializeField] через ScriptableObject.
// Але для поточного масштабу проекту це прийнятний компроміс.
```

Це низькопріоритетне виправлення. Для навчального проекту захардкоджені дві фабрики — допустимо.

### 6.4. DIP: Прибрати fallback в EnemyBuilder

`EnemyBuilder.cs` рядок 50 — якщо `behaviorStrategy == null`, створюється `new ChasePlayerStrategy()`. Це порушує DIP: Builder знає про конкретну стратегію.

**Рішення:** Замість fallback — кидати помилку:

```csharp
public GameObject Build()
{
    if (enemyPrefab == null)
        throw new System.InvalidOperationException("Cannot build enemy: Prefab is not set.");
    if (behaviorStrategy == null)
        throw new System.InvalidOperationException("Cannot build enemy: Behavior strategy is not set.");

    GameObject enemy = Object.Instantiate(enemyPrefab);
    var enemyLogic = enemy.GetComponent<EnemyLogic>();
    if (enemyLogic == null)
        throw new System.InvalidOperationException("Enemy prefab does not have EnemyLogic component.");

    enemyLogic.speed = speed;
    enemyLogic.behaviorStrategy = behaviorStrategy;
    return enemy;
}
```

Фабрики завжди задають стратегію, тому fallback непотрібний.

---

## Крок 7. Прибрати закоментований код

| Файл                  | Що видалити                                                                   |
| --------------------- | ----------------------------------------------------------------------------- |
| `EnemyFactoryBase.cs` | Рядки 51-85 (закоментований старий код фабрик)                                |
| `EnemyLogic.cs`       | Рядки 16-25 (закоментований `SetBehaviorStrategy`)                            |
| `EnemyBuilder.cs`     | Рядок 20 з `EnemyDifficulty.Default` fallback — якщо видаляєте Default з enum |

---

## Крок 8. Оновити тести

Файл `DecoratorTest.cs` видалено (крок 1). Якщо потрібні тести — створити нові для того, що залишилось:

- Тест `EnemyBuilder.Build()` — перевірити що без prefab кидає помилку
- Тест `EnemyBuilder.Build()` — перевірити що без strategy кидає помилку
- Тест `BulletData` — перевірити що damage/speed зберігаються
- Тест `EnemyStats.SpeedByDifficulty` — перевірити що словник містить потрібні значення

---

## Крок 9. Перевірка Unity сцен

Після видалення скриптів перевірити в Unity Editor:

1. **GameManager** — зняти посилання на видалені компоненти
2. **Player** — зняти `PlayerStateManager` компонент, зняти `CheckDeath` компонент
3. **Player** — додати `GameOverHandler` компонент замість `CheckDeath`
4. **Player** — переконатись що `PlayerHealth` має призначений `HealthBar` в Inspector
5. **GameEventManager** — видалити об'єкт зі сцени (або компонент)
6. **Всі вороги** — перевірити що `BulletDetection` працює без змін
7. Вороги з `--EnemyDestroy` або `--EnemySpawner` — зняти ці компоненти якщо вони десь на сцені

---

## Підсумок: що залишається в проекті

```
Assets/Scripts/
├── Bullets/
│   ├── BulletDetection.cs        # Здоров'я ворога + попадання (з isDead фіксом)
│   ├── BulletMovement.cs         # Рух снаряда (BulletData замість IBullet)
│   ├── BulletSpawner.cs          # Ворожий спавнер куль (спрощений)
│   └── BulletSystem.cs           # BulletData (простий клас, без декораторів)
├── Enemy/
│   ├── EnemyBuilder.cs           # Builder (без fallback)
│   ├── EnemyFactoryBase.cs       # Abstract Factory (без коментарів)
│   ├── EnemyLogic.cs             # Strategy consumer (без коментарів)
│   ├── EnemyStats.cs             # Enum + словник
│   ├── EnemyStrategy.cs          # Strategy: Chase + Evade
│   └── PrefabProvider.cs         # IPrefabProvider
├── UI/
│   ├── Billboard.cs              # UI обличчям до камери
│   ├── GameOverHandler.cs        # НОВИЙ: обробка смерті гравця
│   ├── HealthBar.cs              # Slider (без звуку)
│   └── UI_Buttons.cs             # Play, Menu, Quit
├── GameManager.cs                # Singleton (виправлений)
├── Player Movement.cs            # Рух (без State залежностей)
├── PlayerHealth.cs               # Здоров'я (event замість Observer)
├── PlayerShooting.cs             # Стрільба (BulletData замість IBullet)
└── Scripts.asmdef
```

### Патерни, що залишаються:

- **Abstract Factory** — EasyEnemyFactory, HardEnemyFactory
- **Builder** — EnemyBuilder
- **Strategy** — ChasePlayerStrategy, EvadeStrategy
- **Singleton** — GameManager (виправлений)

### Що видалено:

- Decorator (куль) — замінений простим BulletData
- State (гравця) — SpeedBoostState, ShieldState не використовувались
- Observer — ніхто не підписувався, замінений C# event
- Flyweight — overengineering для пари int+float
- Custom Attribute — використовувався тільки в тестах для декораторів
