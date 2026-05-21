# info.md

## Project Overview

Project ini adalah simulator edukasi bertema slot machine yang dibuat menggunakan Unity 2D. Fokus utama project bukan perjudian nyata, melainkan demonstrasi bagaimana outcome pada sistem slot dapat dikontrol menggunakan berbagai pendekatan logika program. Arsitektur project sengaja dibuat modular agar setiap mekanisme manipulasi outcome dapat dipisahkan dari visual reel dan UI.

Project saat ini memiliki 4 mekanisme utama:

1. Database Outcome
2. Pattern Engine
3. RTP Control
4. State Machine

Semua mekanisme menggunakan interface yang sama (`IOutcomeEngine`) agar `GameController` tidak perlu mengetahui detail implementasi internal setiap engine. Tujuan utama pendekatan ini adalah scalability dan observability untuk demonstrasi edukasi.

---

# Design Philosophy

## Separation of Visual and Outcome

Keputusan paling penting dalam arsitektur project ini adalah memisahkan:

- visual reel
- logika outcome

Engine menentukan hasil terlebih dahulu, sedangkan reel hanya memvisualisasikan hasil tersebut. Hal ini disengaja karena target edukasi project adalah menjelaskan bahwa outcome pada slot modern dapat dikontrol secara internal tanpa bergantung pada reel visual.

Flow:

```text
Engine -> menentukan hasil
↓
GameController -> menerima hasil
↓
ReelController -> hanya animasi visual
↓
UI update
```

Bukan:

```text
Random reel visual
↓
Baru dihitung menang/kalah
```

---

# Current Runtime Architecture

## Main Runtime Flow

Saat scene dimulai:

1. Unity instantiate semua GameObject
2. `GameController.Start()` dijalankan
3. Balance awal diupdate ke UI
4. `MechanismSelector.Start()` dijalankan
5. Dropdown mechanism dibaca
6. Engine dibuat sesuai dropdown
7. `GameController.SetEngine()` dipanggil
8. Rule preview disinkronisasi
9. Game siap menerima spin

---

# Scene Hierarchy (IMPORTANT)

Hierarchy berikut penting karena beberapa dependency tidak terlihat dari struktur folder project.

```text
MainScene
├── GameController
├── MechanismController
├── EventSystem
├── Canvas
│   ├── BalanceText
│   ├── ResultText
│   ├── SpinButton
│   ├── MechanismDropdown
│   └── RulePreviewDropdown
│
├── Reel1
├── Reel2
└── Reel3
```

---

# Important Inspector Wiring

## GameController Object

Attached script:
- `GameController.cs`

Inspector references:
- Reel1
- Reel2
- Reel3
- UIController (Canvas)

Jika reference ini hilang, spin flow akan gagal.

---

## MechanismController Object

Attached script:
- `MechanismSelector.cs`

Inspector references:
- MechanismDropdown
- RulePreviewDropdown
- GameController

Object ini sangat penting karena bertindak sebagai sinkronisasi utama antara:
- UI dropdown
- engine internal
- rule preview

Tanpa object ini:
- engine tidak pernah di-set
- `activeEngine` null
- spin tidak berjalan

---

## Canvas Object

Attached script:
- `UIController.cs`

Inspector references:
- BalanceText
- ResultText
- RulePreviewDropdown

Canvas bertanggung jawab untuk seluruh sinkronisasi visual UI.

---

# Engine System

## IOutcomeEngine

Semua engine implement interface berikut:

```csharp
GenerateResult()
GetRules()
GetEngineName()
```

Tujuan:
- GameController tidak perlu mengetahui detail engine
- memungkinkan swapping engine runtime
- mempermudah observability dan debugging

---

# Engine Explanations

## 1. DatabaseOutcomeEngine

Engine paling deterministic.

Outcome sudah disiapkan sebelumnya dalam list:

```text
LOSS
LOSS
SMALL_WIN
LOSS
JACKPOT
```

Engine hanya membaca urutan menggunakan `currentIndex`.

Saat index habis:
- reset ke 0
- loop ulang

Tidak ada randomness.

Tujuan:
- demonstrasi scripted outcome
- debugging mudah
- validasi visual

---

## 2. PatternOutcomeEngine

Menggunakan rule berbasis pola.

Contoh:
- setiap 10 spin -> small win
- setiap 25 spin -> jackpot
- setelah loss streak tertentu -> recovery win

Engine menyimpan internal state:
- `spinCount`
- `lossStreak`

Tujuan:
- menunjukkan manipulasi berbasis perilaku pengguna
- lebih realistis dibanding scripted sequence

---

## 3. RTPControlEngine

Mensimulasikan Return To Player control.

State internal:
- `totalBet`
- `totalPayout`
- `targetRTP`

Formula:

```text
RTP = totalPayout / totalBet
```

Engine membandingkan RTP aktual dengan target RTP lalu memutuskan apakah pemain akan diberikan kemenangan.

Dropdown rule preview berubah menjadi RTP selector:
- 60%
- 75%
- 90%

Tujuan:
- menjelaskan konsep house edge
- menunjukkan balancing payout otomatis

---

## 4. StateMachineEngine

Engine berbasis mode internal.

Available states:
- Cold
- Normal
- Hot
- Bonus

Setiap state memiliki perilaku berbeda.

Contoh:
- Cold -> hampir selalu kalah
- Hot -> win rate tinggi
- Bonus -> jackpot bias
- Normal -> mixed behavior

RulePreviewDropdown berubah fungsi menjadi state selector.

Tujuan:
- simulasi adaptive machine behavior
- demonstrasi stateful manipulation

---

# UI Behavior

## MechanismDropdown

Selector utama engine.

Options:
- Database Outcome
- Pattern Engine
- RTP Control
- State Machine

Dropdown ini adalah single source of truth untuk engine aktif.

---

## RulePreviewDropdown

Behavior dinamis.

### Database / Pattern
Read-only preview.

### RTP
Interactive RTP selector.

### State Machine
Interactive state selector.

Karena behavior berubah berdasarkan engine aktif, AI agent yang melanjutkan project harus berhati-hati agar tidak mengasumsikan dropdown ini selalu read-only.

---

# Reel System

Current reel implementation masih placeholder.

Saat ini reel:
- hanya rotate visual
- belum memakai symbol strip asli
- belum sinkron dengan simbol kemenangan

Hal ini disengaja untuk memprioritaskan engine architecture terlebih dahulu.

Planned future upgrade:
- symbol strip
- weighted symbol
- fake reel stop
- controlled symbol mapping

---

# Debug Logging

Project memiliki debug log cukup penting.

Log digunakan untuk:
- active engine tracking
- payout tracing
- RTP observation
- state observation

Console log termasuk:
- spin start
- engine switch
- generated outcome
- payout applied
- state machine state
- RTP values

Jika AI agent menambah fitur baru, disarankan mempertahankan observability ini.

---

# Important Architectural Notes

## Engine Lifecycle

Engine baru dibuat setiap kali mechanism dropdown berubah.

Artinya:
- state lama hilang
- RTP reset
- spin count reset
- loss streak reset

Ini intentional untuk demonstrasi.

Jika ingin persistent state:
- engine instance harus disimpan
- bukan recreate setiap switch

---

## Coroutine Dependency

Spin flow menggunakan coroutine.

Flow:

```text
Spin button
↓
SpinRoutine()
↓
GenerateResult()
↓
Reel animations
↓
Apply payout
↓
UI update
```

Karena sequential coroutine:
- reel spin sekarang berjalan satu per satu
- bukan paralel

Hal ini masih placeholder.

---

# Current Technical Debt

## Reel Visual

Belum ada:
- actual symbol reel
- paylines
- weighted symbols
- stop positions

---

## UI

Masih minimal:
- belum ada proper debug panel
- belum ada animated transitions
- dropdown overload behavior masih cukup hacky

---

## Outcome Representation

`SpinResult` masih sangat sederhana.

Belum mendukung:
- paylines
- multiple symbol matches
- free spins
- bonus multipliers
- reel stop coordinates

---

# Recommended Next Steps

Priority paling masuk akal:

1. reel symbol strip
2. reel stop synchronization
3. paylines
4. proper debug panel
5. serialized config system
6. external JSON rule loading

---

# AI Handoff Notes

Hal paling penting untuk dipahami AI selanjutnya:

1. Visual reel TIDAK menentukan outcome
2. Engine adalah source of truth
3. RulePreviewDropdown bersifat dynamic-role
4. MechanismSelector adalah synchronization hub
5. Current implementation lebih fokus ke architecture daripada polish
6. Banyak visual saat ini masih placeholder

Kesalahan paling berbahaya yang mungkin dilakukan AI lanjutan adalah:
- memindahkan outcome generation ke reel visual
- menghilangkan abstraction engine
- membuat dropdown sinkronisasi menjadi fragmented
- membuat GameController mengetahui detail internal engine

Karena itu abstraction `IOutcomeEngine` sebaiknya dipertahankan.

---

# Review Pass Summary

## Pass 1
Menambahkan hierarchy dan inspector wiring karena tidak terlihat dari folder structure.

## Pass 2
Menambahkan runtime flow startup agar AI lain memahami initialization order.

## Pass 3
Menambahkan behavioral explanation untuk RulePreviewDropdown karena fungsinya dinamis.

## Pass 4
Menambahkan lifecycle warning bahwa engine di-reset saat switch mechanism.

## Pass 5
Menambahkan AI handoff warning untuk mencegah architectural regression.

Tidak ada perubahan besar tambahan setelah pass kelima.

# Additional Review Pass 6

## Unity Inspector Defaults

Beberapa behavior project bergantung pada nilai default di Inspector, bukan kode.

### MechanismDropdown

Expected default:
```text
Value = 0
Option 0 = Database Outcome
```

Karena `MechanismSelector.Start()` membaca:

```csharp
mechanismDropdown.value
```

Jika urutan option berubah di Inspector tanpa update switch-case:
- engine mismatch
- dropdown mismatch
- rule preview mismatch

AI berikutnya sebaiknya tidak mengubah urutan option dropdown tanpa refactor selector logic.

---

## RulePreviewDropdown Interactable State

Dropdown ini berubah interactable state berdasarkan mechanism aktif.

Behavior expected:

### Database Outcome
```text
interactable = false
```

### Pattern Engine
```text
interactable = false
```

### RTP Control
```text
interactable = true
```

### State Machine
```text
interactable = true
```

Jika dropdown accidentally tetap interactable pada mode readonly:
- user bisa memilih option
- callback tetap terpanggil
- berpotensi trigger logic tidak diinginkan

---

## Current Spin Timing Behavior

Saat ini reel spin berjalan sequential:

```text
Reel1 selesai
↓
Reel2 selesai
↓
Reel3 selesai
```

Bukan paralel.

Ini menyebabkan total spin duration lebih panjang dari slot normal.

Behavior ini bukan bug, tetapi placeholder simplification.

Jika AI berikutnya ingin membuat behavior realistis:
- gunakan parallel coroutine
- atau staggered stop timing

Namun outcome generation tetap harus dilakukan sebelum animasi dimulai.

---

## Current Reel Representation

Reel object saat ini sebenarnya belum merepresentasikan reel slot sesungguhnya.

Current implementation:
```text
single transform rotation
```

Belum ada:
- reel strip
- visible window
- symbol indexing
- stop position mapping

Karena itu AI lain jangan mengasumsikan reel memiliki internal symbol state.

---

## Hidden Coupling

Walaupun architecture sudah modular, masih ada beberapa hidden coupling:

### MechanismSelector ↔ engine names

Logic berikut:

```csharp
gameController.GetCurrentEngineName()
```

masih bergantung pada string comparison:

```text
"RTP Control"
"State Machine"
```

Ini technically fragile.

Refactor future yang lebih baik:
- enum engine type
- polymorphic config panel
- interface capability check

Namun untuk scope sekarang masih acceptable.

---

# Additional Review Pass 7

## State Persistence Clarification

Saat mechanism berubah:

```text
Database -> RTP
```

engine lama dihancurkan secara implicit karena object baru dibuat.

Akibatnya:
- RTP statistics reset
- pattern counters reset
- state machine state reset

Current project tidak memiliki centralized save state manager.

Jika AI agent menambahkan:
- persistent sessions
- analytics
- replay system

maka perlu abstraction baru untuk:
- runtime state persistence
- engine snapshots
- serialization

---

## Potential Future Serialization Path

Arsitektur saat ini sudah cukup compatible untuk future external config loading.

Candidate future config:
- JSON pattern rules
- RTP presets
- scripted outcome databases
- state transition tables

Recommended future structure:

```text
Assets/
├── StreamingAssets/
│   ├── Patterns/
│   ├── RTP/
│   └── States/
```

Namun current implementation masih hardcoded untuk menjaga debugging tetap sederhana.

---

## Debugging Recommendation

Saat development lanjut, sangat disarankan menambahkan visual debug panel dibanding hanya `Debug.Log`.

Suggested future debug panel:
- current engine
- current RTP
- spin count
- loss streak
- active state
- total payout
- total bet

Karena simulator ini bersifat edukasi internal-state visibility sangat penting.

---

## Known Simplifications

Project saat ini sengaja menyederhanakan beberapa konsep slot machine:

### No paylines
Belum ada evaluasi horizontal/diagonal line.

### No weighted symbols
Semua hasil ditentukan engine langsung.

### No reel physics
Reel tidak memiliki inertia/acceleration sesungguhnya.

### No asynchronous economy
Tidak ada delayed payout processing.

### No animation-state sync
UI result muncul setelah spin selesai tanpa event-based animation timing.

Ini intentional demi fokus pada educational outcome manipulation architecture.

---

## Safety Boundary Clarification

Walaupun visual menyerupai slot machine, project secara internal didesain sebagai:
- simulator edukasi
- observability tool
- architecture demonstration

Karena itu:
- tidak ada transaksi nyata
- tidak ada networking
- tidak ada persistence economy
- tidak ada monetization flow

Semua economy masih local-memory runtime saja.

---

## Long-Term Refactor Direction

Jika project berkembang besar, kemungkinan perlu memecah `GameController`.

Candidate separation:

```text
GameController
├── SpinManager
├── EconomyManager
├── EngineManager
├── ReelManager
└── UIManager
```

Namun untuk current scale:
- single GameController masih acceptable
- complexity belum cukup tinggi untuk ECS/event-bus architecture

Premature overengineering sebaiknya dihindari.