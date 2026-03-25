# projectA

## Script.Core — Documentação

Namespace `Script.Core` contém toda a lógica central do sistema de mapa roguelike. Os scripts ficam em `Assets/Script/Core/`.

---

## Visão geral da arquitetura

```
MapLoader (MonoBehaviour)
 ├── lê o JSON e popula MapData
 ├── delega construção do mapa → MapBuilder
 └── delega spawn do player    → PlayerSpawner
                                      └── dispara MapEvents.OnPlayerSpawned
                                                    ↓
                                             CameraFollow (subscreve)

PlayerController (MonoBehaviour)
 └── consulta vizinhos → MapBuilder.GetNeighbors()
```

---

## MapData.cs

Modelos de dados desserializados a partir do JSON do mapa. Todos os tipos são marcados com `[Serializable]` para compatibilidade com `JsonUtility`.

### `enum NodeType`

Representa o tipo de cada nó do mapa.

| Valor       | Int | Descrição                        |
|-------------|-----|----------------------------------|
| `Start`     | 0   | Ponto de entrada do player       |
| `Combat`    | 1   | Encontro de combate              |
| `Shop`      | 2   | Loja de itens                    |
| `Rest`      | 3   | Ponto de descanso / cura         |
| `Treasure`  | 4   | Recompensa / tesouro             |
| `End`       | 5   | Fim do mapa / boss               |

---

### `class NodeData`

Dados de um único nó, lidos do JSON.

| Campo   | Tipo       | Descrição                                      |
|---------|------------|------------------------------------------------|
| `id`    | `int`      | Identificador único do nó                      |
| `x`     | `float`    | Posição horizontal no espaço lógico do mapa    |
| `y`     | `float`    | Posição vertical no espaço lógico do mapa      |
| `type`  | `NodeType` | Categoria do nó (Start, Combat, etc.)          |
| `label` | `string`   | Nome exibido no nó (ex: "Floresta Sombria")    |

---

### `class ConnectionData`

Representa uma aresta (conexão) entre dois nós.

| Campo  | Tipo  | Descrição               |
|--------|-------|-------------------------|
| `from` | `int` | ID do nó de origem      |
| `to`   | `int` | ID do nó de destino     |

As conexões são bidirecionais na prática — `GetNeighbors` trata ambas as direções.

---

### `class MapData`

Raiz do JSON do mapa.

| Membro        | Tipo                            | Descrição                                     |
|---------------|---------------------------------|-----------------------------------------------|
| `mapName`     | `string`                        | Nome do mapa                                  |
| `nodes`       | `List<NodeData>`                | Lista bruta de nós (usada pelo `JsonUtility`) |
| `connections` | `List<ConnectionData>`          | Lista bruta de conexões                       |
| `Nodes`       | `IReadOnlyList<NodeData>`       | Acesso somente leitura aos nós                |
| `Connections` | `IReadOnlyList<ConnectionData>` | Acesso somente leitura às conexões            |

> `nodes` e `connections` precisam ser `public` para o `JsonUtility` desserializar corretamente. Use sempre as propriedades `Nodes` e `Connections` no código para evitar modificações acidentais.

**Exemplo de JSON esperado (`StreamingAssets/Maps/`):**
```json
{
  "mapName": "Dungeon 1",
  "nodes": [
    { "id": 0, "x": 0, "y": 0, "type": 0, "label": "Entrada" },
    { "id": 1, "x": 2, "y": 1, "type": 1, "label": "Combate" }
  ],
  "connections": [
    { "from": 0, "to": 1 }
  ]
}
```

---

## MapLoader.cs

`MonoBehaviour` responsável por **carregar o JSON** e orquestrar a construção do mapa.

### Campos públicos (Inspector)

| Campo          | Tipo         | Descrição                                                  |
|----------------|--------------|------------------------------------------------------------|
| `mapFileName`  | `string`     | Nome do arquivo JSON dentro de `StreamingAssets/Maps/`     |
| `nodePrefab`   | `GameObject` | Prefab instanciado para cada nó                            |
| `linePrefab`   | `GameObject` | Prefab com `LineRenderer` usado para desenhar conexões     |
| `playerPrefab` | `GameObject` | Prefab do player (opcional — cria um padrão se nulo)       |
| `scale`        | `float`      | Multiplicador aplicado às coordenadas `x`/`y` dos nós     |

### Ciclo de vida

- **`Start()`** — chama `LoadMap()` automaticamente ao iniciar a cena.
- **`LoadMap()`** — lê o arquivo JSON, desserializa para `MapData`, repassa para `MapBuilder` e `PlayerSpawner`. Em caso de erro retorna cedo sem travar.
- **`ReloadMap()`** — destroi todos os filhos do transform, limpa o estado do `MapBuilder` e recarrega o mapa do zero. Pode ser chamado externamente (ex: botão de debug).

### Tratamento de erros

| Situação                    | Comportamento                                                       |
|-----------------------------|---------------------------------------------------------------------|
| Arquivo JSON não encontrado | `Debug.LogError("[MapLoader] Erro ao ler arquivo '...'")` + return  |
| JSON malformado / inválido  | `Debug.LogError("[MapLoader] JSON inválido em '...'")` + return     |

---

## MapBuilder.cs

Classe C# pura (não `MonoBehaviour`) responsável por **instanciar nós e conexões** na cena a partir de um `MapData`.

### Propriedades

| Propriedade | Tipo                                | Descrição                                       |
|-------------|-------------------------------------|-------------------------------------------------|
| `Nodes`     | `IReadOnlyDictionary<int, MapNode>` | Mapa de ID → componente `MapNode` vivo na cena  |

### Métodos

#### `Build(MapData, GameObject nodePrefab, GameObject linePrefab, Transform parent, float scale)`

Constrói o mapa visual completo:
1. Para cada `NodeData`, instancia o `nodePrefab`, posiciona em `(x * scale, y * scale, 0)`, nomeia como `Node_{id}_{label}` e chama `MapNode.Init(nodeData)`.
2. Para cada `ConnectionData`, instancia o `linePrefab` como filho do transform pai e configura o `LineRenderer` com as posições dos dois nós.
3. Registra os nós no dicionário interno `_nodes`.

#### `Clear()`

Limpa o dicionário `_nodes`. Deve ser chamado antes de recarregar o mapa para evitar referências obsoletas.

#### `GetNeighbors(int nodeId, IReadOnlyList<ConnectionData> connections)`

Retorna todos os `MapNode` vizinhos de um dado nó, percorrendo a lista de conexões e checando ambas as direções (`from == nodeId` ou `to == nodeId`).

---

## MapNode.cs

`MonoBehaviour` que vive em cada prefab de nó instanciado. Responsável pela **apresentação visual** do nó.

### Campos públicos (Inspector)

Cada tipo de nó tem um sprite e uma cor configuráveis pelo Inspector:

| Campo            | Tipo     | Tipo de nó  |
|------------------|----------|-------------|
| `spriteStart`    | `Sprite` | Start       |
| `spriteCombat`   | `Sprite` | Combat      |
| `spriteShop`     | `Sprite` | Shop        |
| `spriteRest`     | `Sprite` | Rest        |
| `spriteTreasure` | `Sprite` | Treasure    |
| `spriteEnd`      | `Sprite` | End         |
| `colorStart`     | `Color`  | Start       |
| `colorCombat`    | `Color`  | Combat      |
| `colorShop`      | `Color`  | Shop        |
| `colorRest`      | `Color`  | Rest        |
| `colorTreasure`  | `Color`  | Treasure    |
| `colorEnd`       | `Color`  | End         |

As cores servem de fallback quando nenhum sprite está atribuído.

### Propriedade

| Propriedade | Tipo       | Descrição                                  |
|-------------|------------|--------------------------------------------|
| `Data`      | `NodeData` | Dados do nó (somente leitura externamente) |

### Método

#### `Init(NodeData data)`

Inicializa o nó:
1. Armazena `data` em `Data`.
2. Obtém o `SpriteRenderer` do GameObject.
3. Usa uma expressão `switch` para atribuir o sprite e a cor correspondente ao `NodeType`.
4. Atualiza o texto do componente `TMP_Text` filho (se existir) com `data.label`.

---

## PlayerSpawner.cs

Classe C# pura responsável por **encontrar o nó de início, instanciar o player e disparar o evento de spawn**.

### Método

#### `Spawn(MapData, IReadOnlyDictionary<int, MapNode>, GameObject playerPrefab, MapBuilder)`

1. Encontra o `NodeData` com `type == NodeType.Start` via LINQ `FirstOrDefault`.
2. Instancia o `playerPrefab` na posição do nó de início. Se `playerPrefab` for `null`, cria um `GameObject` com `SpriteRenderer` padrão (quadrado branco/azul claro, 32×32 px).
3. Garante que o player tenha um componente `PlayerController`, adicionando-o dinamicamente se necessário.
4. Chama `PlayerController.Init()` passando o ID do nó inicial, a referência ao `MapBuilder` e a lista de conexões.
5. Garante que a câmera principal tenha um componente `CameraFollow`, adicionando-o se ausente.
6. Dispara `MapEvents.OnPlayerSpawned(player.transform)`.

---

## MapEvents.cs

Classe estática que serve de **event bus** para comunicação desacoplada entre componentes.

### Eventos

| Evento            | Assinatura          | Quando é disparado                     |
|-------------------|---------------------|----------------------------------------|
| `OnPlayerSpawned` | `Action<Transform>` | Imediatamente após o player ser criado |

**Quem dispara:** `PlayerSpawner.Spawn()`
**Quem escuta:** `CameraFollow` (via `OnEnable` / `OnDisable`)

---

## PlayerController.cs

`MonoBehaviour` responsável pelo **movimento do player** pelo mapa.

### Campos públicos (Inspector)

| Campo          | Tipo    | Descrição                                       |
|----------------|---------|-------------------------------------------------|
| `moveDuration` | `float` | Duração em segundos de cada movimento entre nós |

### Método

#### `Init(int startNodeId, MapBuilder mapBuilder, IReadOnlyList<ConnectionData> connections)`

Deve ser chamado por `PlayerSpawner` antes do primeiro `Update`. Define o nó atual, a referência ao `MapBuilder` e a lista de conexões usada para consultar vizinhos.

### Ciclo de vida

- **`Update()`** — detecta input do teclado (W/A/S/D via `UnityEngine.InputSystem`) e chama `TryMove(direction)` se o player não estiver em movimento.
- **`TryMove(Vector2 direction)`** — obtém os vizinhos do nó atual via `MapBuilder.GetNeighbors()`, calcula o `dot product` entre a direção do input e o vetor até cada vizinho, e inicia o movimento para o vizinho com maior alinhamento (desde que `dot > 0`).
- **`MoveToNode(MapNode target)`** (corrotina) — interpola a posição do player de `startPos` até `target.transform.position` usando `Vector3.Lerp` ao longo de `moveDuration` segundos. Atualiza `_currentNodeId` no início do movimento.

### Lógica de navegação

O player nunca "anda para trás" — apenas vizinhos com `dot product > 0` em relação à direção pressionada são candidatos. O vizinho com o maior `dot` vence.

---

## CameraFollow.cs

`MonoBehaviour` que deve estar na câmera principal. Segue o player suavemente após receber o evento de spawn.

### Campos públicos (Inspector)

| Campo         | Tipo    | Descrição                                   |
|---------------|---------|---------------------------------------------|
| `smoothSpeed` | `float` | Fator de suavização do `Lerp` (padrão: `5`) |

### Ciclo de vida

- **`OnEnable()`** — subscreve ao `MapEvents.OnPlayerSpawned`.
- **`OnDisable()`** — cancela a subscrição (evita callbacks em objetos destruídos).
- **`SetTarget(Transform t)`** — callback interno; armazena o transform do player em `_target`.
- **`LateUpdate()`** — a cada frame, interpola a posição da câmera em direção ao player em X e Y, preservando a coordenada Z original da câmera.

> O componente pode estar na câmera desde a configuração da cena ou ser adicionado dinamicamente por `PlayerSpawner`. Em ambos os casos a subscrição ao evento garante que `_target` será preenchido corretamente.

---

## Fluxo completo de inicialização

```
Cena carrega
  └─ MapLoader.Start()
       └─ MapLoader.LoadMap()
            ├─ File.ReadAllText(StreamingAssets/Maps/<arquivo>.json)
            ├─ JsonUtility.FromJson<MapData>(json)
            ├─ MapBuilder.Build(mapData, ...)
            │    ├─ Instantiate nodePrefab × N  →  MapNode.Init()
            │    └─ Instantiate linePrefab × M  →  LineRenderer configurado
            └─ PlayerSpawner.Spawn(mapData, nodes, playerPrefab, mapBuilder)
                 ├─ Encontra NodeType.Start
                 ├─ Instantiate player
                 ├─ PlayerController.Init(startId, mapBuilder, connections)
                 ├─ Adiciona CameraFollow na câmera (se ausente)
                 └─ MapEvents.OnPlayerSpawned?.Invoke(player.transform)
                                                        ↓
                                               CameraFollow.SetTarget(t)
```
