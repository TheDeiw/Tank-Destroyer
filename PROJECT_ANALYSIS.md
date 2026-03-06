# Тотальний аналіз проекту MapzLabs (Unity Tank Game)

## Зміст

1. [Загальний огляд](#загальний-огляд)
2. [Структура проекту](#структура-проекту)
3. [Опис кожного скрипта](#опис-кожного-скрипта)
4. [Реалізовані патерни проектування](#реалізовані-патерни-проектування)
5. [Аналіз SOLID принципів](#аналіз-solid-принципів)
6. [Що реалізовано, але НЕ використовується](#що-реалізовано-але-не-використовується)
7. [Що можна видалити](#що-можна-видалити)
8. [Критичні баги](#критичні-баги)
9. [Чи потрібні всі патерни?](#чи-потрібні-всі-патерни)
10. [Два файли .sln](#два-файли-sln)
11. [Підсумкова таблиця](#підсумкова-таблиця)

---

## Загальний огляд

Проект — **3D гра про танки** на **Unity** (C#). Гравець керує танком від третьої особи, стріляє по ворожим танкам, які спавняться хвилями. Вороги мають різну складність (Easy/Hard) з різною поведінкою (переслідування або втеча). Гравець має health bar, стани (нормальний, швидкість, щит) та систему снарядів з можливістю модифікації через декоратор.

### Стек технологій

- Unity (3D, CharacterController-based рух)
- C#
- NUnit (Edit Mode тести)
- Cinemachine (камера)

---

## Структура проекту

```
Assets/Scripts/
├── Bullets/
│   ├── BulletDetection.cs        # Здоров'я ворога + обробка попадань
│   ├── BulletEffectAttribute.cs  # Custom attribute для метаданих снарядів
│   ├── BulletMovement.cs         # Рух снаряда (залежить від IBullet)
│   ├── BulletSpawner.cs          # Спавнер снарядів для ворогів
│   └── BulletSystem.cs           # IBullet, StandardBullet, Decorator, Factory, Flyweight
├── Enemy/
│   ├── --EnemyDestroy.cs         # [СТАРИЙ] Стрільба гравця лазером (замінено)
│   ├── --EnemySpawner.cs         # [СТАРИЙ] Ручний спавн ворогів (замінено)
│   ├── EnemyBuilder.cs           # Builder pattern для створення ворогів
│   ├── EnemyFactoryBase.cs       # Abstract Factory: EasyEnemyFactory, HardEnemyFactory
│   ├── EnemyLogic.cs             # Поведінка ворога під час гри (використовує Strategy)
│   ├── EnemyStats.cs             # Enum EnemyDifficulty + словник швидкостей
│   ├── EnemyStrategy.cs          # Strategy: ChasePlayerStrategy, EvadeStrategy
│   └── PrefabProvider.cs         # IPrefabProvider для доступу до префабів
├── Observer/
│   ├── GameEventManager.cs       # Singleton менеджер подій
│   ├── Observer.cs               # ISubscriber<T> + структури подій (EventArgs)
│   └── Publisher.cs              # GameEventPublisher<T> — generic publisher
├── UI/
│   ├── Billboard.cs              # UI елемент завжди повернутий до камери
│   ├── HealthBar.cs              # Slider-based health bar
│   └── UI_Buttons.cs             # Кнопки: Play, Menu, Quit
├── CheckDeath.cs                 # [СТАРИЙ] Перевірка смерті через slider напряму
├── GameManager.cs                # Singleton: спавн ворогів через фабрики, наростання складності
├── Player Movement.cs            # Рух гравця (CharacterController + камера)
├── PlayerHealth.cs               # Здоров'я гравця з інтеграцією Observer
├── PlayerShooting.cs             # Стрільба гравця снарядами
├── PlayerState.cs                # State pattern: Normal, SpeedBoost, Shield
├── PlayerStateManager.cs         # State machine для станів гравця
└── Scripts.asmdef                # Assembly Definition

Assets/Tests/EditMode/
├── DecoratorTest.cs              # Unit тести для Decorator + Factory + Attribute
└── EditMode.asmdef               # Assembly Definition для тестів
```

---

## Опис кожного скрипта

### Bullets/

**BulletSystem.cs** — центральний файл системи снарядів. Містить:

- `BulletConfiguration` — Flyweight: зберігає damage/speed, кешується у фабриці
- `IBullet` — інтерфейс снаряда (Shoot, Damage, Speed, Direction)
- `StandardBullet` — базовий снаряд
- `BulletDecorator` — абстрактний декоратор
- `DamageDecorator` — збільшує damage
- `SpeedDecorator` — множить швидкість
- `BulletFactory` — статична фабрика з кешем конфігурацій (Flyweight)

**BulletMovement.cs** — MonoBehaviour, отримує `IBullet` через `SetBullet()`, рухає снаряд через `Update()`, знищує при зіткненні зі стіною.

**BulletSpawner.cs** — MonoBehaviour для ворожих танків. Через `InvokeRepeating` кожні 1.5с стріляє в гравця. Використовує `BulletFactory.CreateStandardBullet()`.

**BulletDetection.cs** — MonoBehaviour для ворогів. Керує здоров'ям ворога, реагує на попадання снарядів (`OnTriggerEnter`), показує HealthBar, створює вибух при смерті.

**BulletEffectAttribute.cs** — Custom `[Attribute]` для анотації класів снарядів метаданими (тип ефекту, опис, пріоритет). Використовується лише для тестів і рефлексії.

### Enemy/

**EnemyStrategy.cs** — Strategy pattern:

- `IEnemyBehaviorStrategy` — інтерфейс з методом `Execute()`
- `ChasePlayerStrategy` — рух до гравця
- `EvadeStrategy` — рух від гравця

**EnemyLogic.cs** — MonoBehaviour на ворожому танку. Знаходить гравця, викликає `behaviorStrategy.Execute()` у `Update()`.

**EnemyBuilder.cs** — Builder pattern: `SetPrefab()`, `SetSpeed()`, `SetBehavior()`, `Build()`. Створює ворога з компонентом `EnemyLogic`.

**EnemyFactoryBase.cs** — Abstract Factory:

- `EnemyFactory` — інтерфейс
- `EnemyFactoryBase` — абстрактний клас, використовує `EnemyBuilder`
- `EasyEnemyFactory` — повільний ворог з EvadeStrategy
- `HardEnemyFactory` — швидкий ворог з ChasePlayerStrategy

**EnemyStats.cs** — `EnemyDifficulty` enum + статичний словник `SpeedByDifficulty`.

**PrefabProvider.cs** — `IPrefabProvider` інтерфейс + MonoBehaviour реалізація. Зберігає масив префабів, повертає за індексом.

**--EnemyDestroy.cs** — ЗАСТАРІЛИЙ. Стара стрільба гравця лазером (миттєве знищення ворога). Замінений на `PlayerShooting.cs`.

**--EnemySpawner.cs** — ЗАСТАРІЛИЙ. Ручний спавн ворога по натисканню `E`. Замінений на автоматичний спавн у `GameManager.cs`.

### Observer/

**Observer.cs** — Визначає:

- `ISubscriber<TContext>` — generic інтерфейс підписника
- `PlayerHealthChangedEventArgs` — дані про зміну здоров'я
- `EnemyDiedEventArgs` — дані про смерть ворога
- `PlayerDiedEventArgs` — подія смерті гравця

**Publisher.cs** — `GameEventPublisher<TContext>` — generic publisher. Зберігає список підписників, розсилає повідомлення, автоматично прибирає знищені MonoBehaviour з підписки.

**GameEventManager.cs** — Singleton з `DontDestroyOnLoad`. Містить три publisher'и: `EnemyDiedPublisher`, `PlayerHealthChangedPublisher`, `PlayerDiedPublisher`.

### Player

**Player Movement.cs** — Рух через `CharacterController`, обертання відносно камери, підтримка анімацій через `Animator`, гравітація, ground check.

**PlayerHealth.cs** — Здоров'я гравця. Використовує Observer: при зміні HP нотифікує `PlayerHealthChangedPublisher`, при смерті — `PlayerDiedPublisher`.

**PlayerShooting.cs** — Стрільба снарядами. Автоприціл на ближнього ворога, fire rate обмеження, інстанціація снарядів через `BulletFactory`.

**PlayerState.cs** — State pattern:

- `IPlayerState` — інтерфейс (OnEnter, OnUpdate, OnExit, HandleDamage, GetSpeedMultiplier)
- `NormalPlayerState` — стандартний стан
- `SpeedBoostState` — прискорення на час
- `ShieldState` — блокує damage на час

**PlayerStateManager.cs** — MonoBehaviour, state machine. Зберігає поточний стан, делегує `Update()` і damage-handling стану.

### Інше

**CheckDeath.cs** — ЗАСТАРІЛИЙ. Перевіряє `healthBar.slider.value <= 0` у `Update()` і показує losepanel. Дублює функціонал Observer (PlayerDiedPublisher).

**GameManager.cs** — Singleton, спавнить ворогів через `EasyEnemyFactory` / `HardEnemyFactory` у корутині з наростаючою частотою.

### UI

**Billboard.cs** — `LateUpdate()`: повертає UI елемент (наприклад health bar) обличчям до камери.

**HealthBar.cs** — Slider + Gradient + fill Image. Методи `SetHealth()`, `SetMaxHealth()`. Грає звук вибуху при отриманні шкоди.

**UI_Buttons.cs** — Три методи: PlayGame (сцена 1), ToMenu (сцена 0), Quit.

### Тести

**DecoratorTest.cs** — NUnit Edit Mode тести:

- Тести BulletEffectAttribute (конструктор, валідація, AttributeUsage)
- Тести BulletConfiguration
- Тести StandardBullet (створення, Shoot, атрибути)
- Тести DamageDecorator та SpeedDecorator (модифікація, атрибути)
- Тести BulletFactory (створення різних типів, кешування)
- Тест комбінованих декораторів

---

## Реалізовані патерни проектування

### 1. Decorator (Bullets)

**Файли:** `BulletSystem.cs`
**Суть:** `IBullet` -> `StandardBullet`, обгортки `DamageDecorator`, `SpeedDecorator`.
**Використання в runtime:** Тільки `BulletFactory.CreateStandardBullet()` (в `PlayerShooting` та `BulletSpawner`). Декоратори `CreateEnhancedBullet()` та `CreateFastBullet()` — закоментовані в місцях виклику.
**В тестах:** Повноцінно протестовано.
**Вердикт:** Патерн реалізовано правильно, є unit тести. В грі використовується тільки базовий снаряд — декоратори готові, але не активовані.

### 2. Abstract Factory (Enemies)

**Файли:** `EnemyFactoryBase.cs`
**Суть:** `EnemyFactory` -> `EnemyFactoryBase` -> `EasyEnemyFactory`, `HardEnemyFactory`.
**Використання:** `GameManager` тримає масив фабрик, рандомно обирає при спавні.
**Вердикт:** Повноцінно використовується, потрібен.

### 3. Builder (Enemies)

**Файли:** `EnemyBuilder.cs`
**Суть:** Fluent API для побудови ворога: `SetPrefab().SetSpeed().SetBehavior().Build()`.
**Використання:** Всередині фабрик (`EasyEnemyFactory`, `HardEnemyFactory`).
**Вердикт:** Використовується разом з Factory. Для двох типів ворогів з трьома параметрами Builder дещо надмірний (достатньо було б конструктора), але робить код зрозумілим і розширюваним.

### 4. Strategy (Enemy Behavior)

**Файли:** `EnemyStrategy.cs`
**Суть:** `IEnemyBehaviorStrategy` -> `ChasePlayerStrategy`, `EvadeStrategy`.
**Використання:** `EnemyLogic` делегує рух стратегії в `Update()`. Фабрики задають стратегію: Hard — Chase, Easy — Evade.
**Вердикт:** Повноцінно використовується, потрібен. Дає реальну різницю в поведінці ворогів.

### 5. State (Player)

**Файли:** `PlayerState.cs`, `PlayerStateManager.cs`
**Суть:** `IPlayerState` -> `NormalPlayerState`, `SpeedBoostState`, `ShieldState`.
**Використання:** `PlayerStateManager` створює `NormalPlayerState` на старті. `SpeedBoostState` та `ShieldState` **ніде не активуються** — немає коду, який би переключав стан (наприклад при підборі бонусу).
**Вердикт:** Патерн реалізовано, але не підключено до геймплею. `SpeedBoostState` та `ShieldState` — мертвий код.

### 6. Observer (Events)

**Файли:** `Observer.cs`, `Publisher.cs`, `GameEventManager.cs`
**Суть:** Типизований Observer: `ISubscriber<T>`, `GameEventPublisher<T>`, три канали подій.
**Використання:** `PlayerHealth` нотифікує `PlayerHealthChangedPublisher` та `PlayerDiedPublisher`. Але **жоден підписник не реалізує `ISubscriber<T>`** в наявному коді — ніхто не підписується на ці події.
**Вердикт:** Publisher працює, але підписники відсутні. `EnemyDiedPublisher` взагалі не використовується.

### 7. Singleton

**Файли:** `GameManager.cs`, `GameEventManager.cs`

- `GameEventManager` — коректний Singleton з `DontDestroyOnLoad`.
- `GameManager` — **зламаний** Singleton (див. Критичні баги).

### 8. Flyweight (BulletFactory)

**Файли:** `BulletSystem.cs` (`BulletFactory.configCache`)
**Суть:** Кешування `BulletConfiguration` за ключем.
**Вердикт:** Технічно реалізовано, але надмірно. `BulletConfiguration` — легкий об'єкт (int + float), який створюється рідко. Кешування не дає помітної переваги.

### 9. Custom Attribute

**Файли:** `BulletEffectAttribute.cs`
**Суть:** Анотація класів снарядів метаданими (тип ефекту, опис, пріоритет).
**Використання:** Читається тільки в тестах через рефлексію. В runtime не використовується для жодної логіки.
**Вердикт:** Демонстраційний елемент для лабораторної (тестування custom attributes через рефлексію).

---

## Аналіз SOLID принципів

### S — Single Responsibility Principle

| Компонент               | Вердикт   | Коментар                                                                                                 |
| ----------------------- | --------- | -------------------------------------------------------------------------------------------------------- |
| `BulletSystem.cs`       | Порушено  | Один файл містить 7 класів/інтерфейсів. Кожен клас окремо дотримується SRP, але організація коду змішана |
| `BulletDetection.cs`    | Порушено  | Відповідає за здоров'я ворога, обробку попадань, перевірку смерті в Update(), і створення ефекту вибуху  |
| `PlayerShooting.cs`     | Порушено  | Містить логіку стрільби І обертання башти — дві відповідальності                                         |
| `PlayerHealth.cs`       | Дотримано | Тільки управління здоров'ям + нотифікація через Observer                                                 |
| `EnemyBuilder.cs`       | Дотримано | Тільки побудова ворога                                                                                   |
| `EnemyLogic.cs`         | Дотримано | Тільки делегування поведінки стратегії                                                                   |
| `GameManager.cs`        | Порушено  | Singleton + спавн ворогів + налаштування складності. Змішано управління грою і спавн                     |
| `GameEventPublisher<T>` | Дотримано | Тільки управління підписниками і розсилка                                                                |
| `HealthBar.cs`          | Частково  | Відповідає за UI slider + грає звук вибуху. Звук — не відповідальність health bar                        |
| `PlayerStateManager.cs` | Дотримано | Тільки управління станами                                                                                |
| `UI_Buttons.cs`         | Дотримано | Тільки навігація між сценами                                                                             |

### O — Open/Closed Principle

| Компонент           | Вердикт   | Коментар                                                  |
| ------------------- | --------- | --------------------------------------------------------- |
| Decorator (Bullets) | Дотримано | Легко додати нові декоратори                              |
| Strategy (Enemy)    | Дотримано | Легко додати нову поведінку ворога                        |
| Factory (Enemy)     | Дотримано | Легко додати нову фабрику                                 |
| State (Player)      | Дотримано | Легко додати новий стан                                   |
| Observer            | Дотримано | Легко додати нових підписників                            |
| `BulletFactory`     | Порушено  | Для додавання нового типу снаряда потрібно змінювати клас |
| `GameManager`       | Порушено  | Масив фабрик захардкоджений в Awake()                     |

### L — Liskov Substitution Principle

| Компонент              | Вердикт   | Коментар                                                        |
| ---------------------- | --------- | --------------------------------------------------------------- |
| IBullet реалізації     | Дотримано | StandardBullet, DamageDecorator, SpeedDecorator — взаємозамінні |
| IEnemyBehaviorStrategy | Дотримано | ChasePlayerStrategy, EvadeStrategy — взаємозамінні              |
| IPlayerState           | Дотримано | Всі стани коректно реалізують інтерфейс                         |
| ISubscriber<T>         | Дотримано | Будь-який підписник може замінити інший                         |
| IPrefabProvider        | Дотримано | PrefabProvider коректно реалізує інтерфейс                      |
| EnemyFactory           | Дотримано | EasyEnemyFactory, HardEnemyFactory — взаємозамінні              |

**LSP дотримано скрізь.**

### I — Interface Segregation Principle

| Інтерфейс                | Вердикт   | Коментар                                                                        |
| ------------------------ | --------- | ------------------------------------------------------------------------------- |
| `IBullet`                | Дотримано | 4 члени — все потрібне для снаряда                                              |
| `IEnemyBehaviorStrategy` | Дотримано | Один метод `Execute()`                                                          |
| `ISubscriber<T>`         | Дотримано | Один метод `OnNotify()`                                                         |
| `IPrefabProvider`        | Дотримано | Один метод `GetPrefab()`                                                        |
| `EnemyFactory`           | Дотримано | Один метод `CreateEnemy()`                                                      |
| `IPlayerState`           | Прийнятно | 5 методів, але всі пов'язані зі станом. Для масштабу проекту розділяти не варто |

### D — Dependency Inversion Principle

| Компонент                                | Вердикт   | Коментар                                                   |
| ---------------------------------------- | --------- | ---------------------------------------------------------- |
| `EnemyFactoryBase` -> `IPrefabProvider`  | Дотримано | Залежить від абстракції                                    |
| `EnemyLogic` -> `IEnemyBehaviorStrategy` | Дотримано | Залежить від абстракції                                    |
| `BulletMovement` -> `IBullet`            | Дотримано | Залежить від абстракції                                    |
| `PlayerStateManager` -> `IPlayerState`   | Дотримано | Залежить від абстракції                                    |
| `GameEventPublisher` -> `ISubscriber<T>` | Дотримано | Залежить від абстракції                                    |
| `PlayerShooting` -> `BulletFactory`      | Порушено  | Пряма залежність від статичного класу                      |
| `BulletSpawner` -> `BulletFactory`       | Порушено  | Пряма залежність від статичного класу                      |
| `GameManager` -> конкретні фабрики       | Порушено  | `new EasyEnemyFactory()`, `new HardEnemyFactory()` напряму |
| `EnemyBuilder` -> `ChasePlayerStrategy`  | Порушено  | Дефолтний fallback `new ChasePlayerStrategy()` при null    |

---

## Що реалізовано, але НЕ використовується

### Повністю мертвий код

| Компонент                                         | Що не так                                                                                                                 |
| ------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| `--EnemyDestroy.cs`                               | Стара система стрільби лазером. Повністю замінена `PlayerShooting.cs`. Клас має помилку в назві: `EnemeDestroy`           |
| `--EnemySpawner.cs`                               | Стара система спавну по клавіші E. Повністю замінена `GameManager.cs`                                                     |
| `CheckDeath.cs`                                   | Перевіряє `healthBar.slider.value` в Update(). Дублює Observer систему (`PlayerDiedPublisher`). Два паралельних механізми |
| Верхня половина `BulletSystem.cs`                 | ~115 рядків закоментованого коду — стара версія                                                                           |
| Закоментований код у `EnemyFactoryBase.cs`        | Рядки 60-85 — стара версія фабрик                                                                                         |
| Закоментований код у `EnemyLogic.cs`              | Рядки 16-25 — старий SetBehaviorStrategy                                                                                  |
| Закоментований код у `BulletSpawner.cs`           | Рядки 14-30 — стара версія LaunchBullet                                                                                   |
| `DamageDecorator` / `SpeedDecorator` (в runtime)  | Класи є і тестуються, але `CreateEnhancedBullet()` та `CreateFastBullet()` ніде не викликаються                           |
| `SpeedBoostState` / `ShieldState`                 | Реалізовані, але **ніде не активуються** — немає power-ups чи іншого тригера                                              |
| `EnemyDiedPublisher`                              | Оголошений в GameEventManager, але ніхто не публікує і не підписується                                                    |
| Підписники `PlayerHealthChangedPublisher`         | PlayerHealth публікує подію, але **жоден клас не реалізує `ISubscriber<PlayerHealthChangedEventArgs>`**                   |
| Підписники `PlayerDiedPublisher`                  | PlayerHealth публікує смерть, але **ніхто не підписаний**                                                                 |
| `PlayerHealth._healthBarReferenceForInitialSetup` | SerializeField ніде не читається в коді                                                                                   |
| `BulletEffectAttribute` (runtime)                 | Атрибут ніде не читається в runtime. Тільки в тестах                                                                      |
| `EnemyDifficulty.Default`                         | Використовується як fallback при speed < 0, що ніколи не настане з поточними фабриками                                    |

### Надмірне, але працює

| Компонент                               | Коментар                                                                      |
| --------------------------------------- | ----------------------------------------------------------------------------- |
| `BulletFactory.configCache` (Flyweight) | Кешування BulletConfiguration (int + float) — overengineering                 |
| `EnemyBuilder` для 3 параметрів         | Fluent API для 3 полів — конструктор був би простішим, але Builder не шкодить |

---

## Що можна видалити

### Безпечно видалити (не зламає функціональність)

1. **`--EnemyDestroy.cs`** — повністю замінений `PlayerShooting.cs`
2. **`--EnemySpawner.cs`** — повністю замінений `GameManager.cs`
3. **Закоментований код у `BulletSystem.cs`** (рядки 1-115) — стара версія
4. **Закоментований код у `EnemyFactoryBase.cs`** (рядки 60-85)
5. **Закоментований код у `EnemyLogic.cs`** (рядки 16-25)
6. **Закоментований код у `BulletSpawner.cs`** (рядки 14-30)
7. **`PlayerHealth._healthBarReferenceForInitialSetup`** — SerializeField, який ніде не використовується
8. **`tskt Test Task.sln`** — застарілий .sln файл

### Видалити з рефакторингом (якщо не для демонстрації лаби)

9. **`CheckDeath.cs`** — замінити підписником на `PlayerDiedPublisher`

### НЕ видаляти (потрібно для лабораторних)

- `DamageDecorator`, `SpeedDecorator` — частина демонстрації Decorator + покриті тестами
- `SpeedBoostState`, `ShieldState` — демонстрація State pattern
- `EnemyDiedPublisher` — демонстрація Observer pattern
- `BulletFactory.configCache` (Flyweight) — демонстрація Flyweight
- `BulletEffectAttribute` — демонстрація Custom Attributes + тести

---

## Критичні баги

### 1. GameManager.Awake — створює клон самого себе

```csharp
void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        Instantiate(gameObject);  // BUG: створює клон GameManager!
    }
    else
    {
        Destroy(gameObject);
    }
}
```

`Instantiate(gameObject)` створює копію GameManager. Копія викликає Awake, бачить `Instance != null`, знищує себе. Має бути `DontDestroyOnLoad(gameObject)` (якщо потрібно зберегти між сценами) або просто нічого.

### 2. PlayerStateManager.SetState — NullReferenceException на старті

```csharp
void Start()
{
    SetState(new NormalPlayerState());
}

public void SetState(IPlayerState newState)
{
    currentState.OnExit();  // NullRef: currentState == null при першому виклику!
    currentState = newState;
    currentState.OnEnter(this);
}
```

При першому виклику `SetState()` в `Start()`, `currentState` ще null, тому `currentState.OnExit()` кидає NullReferenceException.

### 3. PlayerMovement — швидкість стану не застосовується

```csharp
float currentSpeed = baseSpeed * (stateManager != null ? stateManager.GetCurrentSpeedMultiplier() : 1f);
// currentSpeed обчислено, але далі використовується поле speed, а не currentSpeed:
controller.Move(moveDirection.normalized * speed * Time.deltaTime);
```

`currentSpeed` обчислюється з урахуванням стану (рядок 44), але в рядку 56 використовується поле `speed` (12f) напряму. Множник стану ігнорується.

### 4. Observer ніхто не слухає

`PlayerHealth` публікує події через `GameEventManager`, але в жодному класі немає `ISubscriber<PlayerHealthChangedEventArgs>` або `ISubscriber<PlayerDiedEventArgs>`. Подія йде в нікуди. Водночас `CheckDeath.cs` перевіряє здоров'я через slider — Observer реалізований, але не підключений.

### 5. BulletDetection — можливе подвійне спрацювання

```csharp
void Update()
{
    if (currentHealth <= 0)
    {
        Instantiate(explosionEffect, ...);
        Destroy(gameObject);
    }
}
```

`Destroy` відкладений до кінця кадру. Якщо за один кадр `Update()` встигне викликатись повторно (малоймовірно, але можливо при дуже низькому FPS), ефект вибуху створюється двічі. Краще додати прапорець `isDead`.

---

## Чи потрібні всі патерни?

| Патерн                              | Потрібен для гри?                           | Потрібен для лаби?       | Рекомендація                     |
| ----------------------------------- | ------------------------------------------- | ------------------------ | -------------------------------- |
| **Decorator** (Bullets)             | Так, якщо активувати різні типи снарядів    | Так                      | Залишити                         |
| **Abstract Factory** (Enemies)      | Так                                         | Так                      | Залишити                         |
| **Builder** (Enemies)               | Допустимо, overengineering для 3 параметрів | Так                      | Залишити                         |
| **Strategy** (Enemy Behavior)       | Так                                         | Так                      | Залишити                         |
| **State** (Player)                  | Так, якщо підключити до геймплею            | Так                      | Залишити, але підключити         |
| **Observer** (Events)               | Так, якщо додати підписників                | Так                      | Залишити, але додати ISubscriber |
| **Singleton**                       | Так                                         | Не для демонстрації      | Виправити баг                    |
| **Flyweight** (BulletFactory cache) | Ні, overengineering                         | Для демонстрації         | Можна спростити                  |
| **Custom Attribute**                | Ні в runtime                                | Для тестування рефлексії | Залишити                         |

---

## Два файли .sln

У проекті два .sln файли:

### `MapzLabs.sln` (основний)

- **Формат:** Visual Studio 2010 (Format Version 11.00)
- **Проекти:** Scripts, EditMode, Assembly-CSharp (3 проекти)
- **Генерація:** Створений Unity автоматично через IDE-інтеграцію. Коли є Assembly Definition файли (`.asmdef`), Unity створює окремий `.csproj` для кожного: `Scripts.csproj` (з `Scripts.asmdef`), `EditMode.csproj` (з `EditMode.asmdef`), та `Assembly-CSharp.csproj`.
- **Статус:** Актуальний, правильний файл.

### `tskt Test Task.sln` (застарілий)

- **Формат:** Visual Studio 2015 (Format Version 12.00)
- **Проекти:** Тільки Assembly-CSharp (1 проект)
- **Генерація:** Був згенерований Unity раніше — до додавання Assembly Definition файлів. Або створений іншою версією IDE-інтеграції.
- **Статус:** Застарілий. Не знає про Scripts та EditMode проекти. Якщо відкрити проект через цей .sln, IDE не побачить розділення на assembly і тести можуть не скомпілюватись.

### Чому їх два?

Unity перегенерує .sln файли при зміні налаштувань IDE або при "Regenerate project files". Unity генерує .sln з ім'ям проекту (папки), а старий файл з іншим ім'ям не видаляє. `tskt Test Task.sln` залишився від початкового стану проекту (або іншої конфігурації), а `MapzLabs.sln` — поточний файл.

**Рекомендація:** `tskt Test Task.sln` можна безпечно видалити. Використовуйте `MapzLabs.sln`.

---

## Підсумкова таблиця

| Аспект               | Оцінка   | Коментар                                                                                                    |
| -------------------- | -------- | ----------------------------------------------------------------------------------------------------------- |
| **SRP**              | 6/10     | BulletDetection, PlayerShooting, GameManager змішують відповідальності                                      |
| **OCP**              | 7/10     | Патерни дають розширюваність, але BulletFactory та GameManager вимагають модифікації                        |
| **LSP**              | 10/10    | Всі абстракції коректно підміняються                                                                        |
| **ISP**              | 9/10     | Інтерфейси компактні і сфокусовані                                                                          |
| **DIP**              | 6/10     | Добре для Enemy, погано для Bullets (статична BulletFactory)                                                |
| **Decorator**        | 7/10     | Правильна реалізація + тести, але в runtime тільки StandardBullet                                           |
| **Abstract Factory** | 9/10     | Повноцінно працює                                                                                           |
| **Builder**          | 8/10     | Працює, трохи надмірний                                                                                     |
| **Strategy**         | 9/10     | Повноцінно працює, дає реальну різницю                                                                      |
| **State**            | 4/10     | Реалізовано, але не підключено + баг NullRef                                                                |
| **Observer**         | 3/10     | Publisher працює, підписників нема                                                                          |
| **Singleton**        | 5/10     | GameEventManager правильний, GameManager зламаний                                                           |
| **Flyweight**        | 5/10     | Працює, overengineering                                                                                     |
| **Тести**            | 8/10     | Покривають Decorator, Factory, Attribute. Немає тестів для Strategy, Builder, Observer                      |
| **Загальна якість**  | **6/10** | Робочий геймплей з фабриками та стратегіями. Observer не підключений, State не активований, є критичні баги |

---

> **Висновок:** Проект — Unity 3D гра про танки з хорошою системою ворогів (Factory + Builder + Strategy повноцінно працюють разом). Система снарядів (Decorator + Flyweight + Custom Attribute) реалізована і протестована, але в грі використовується лише базовий снаряд. Observer технічно правильний (generic publisher, автоочистка знищених MonoBehaviour), але не підключений до жодного підписника. State pattern реалізований, але стани SpeedBoost та Shield ніде не активуються. Є кілька критичних багів (GameManager створює клон себе, PlayerStateManager NullRef на старті, швидкість стану ігнорується). Застарілі файли (`--EnemyDestroy.cs`, `--EnemySpawner.cs`, `CheckDeath.cs`) варто видалити або замінити Observer-підписниками.
