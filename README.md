# Backgammon

A C# WPF implementation of the classic **Backgammon** game.  
The rules, board layout, and visual style are based on the version available at:  
https://www.kurnik.pl/tryktrak/

Key rules summarized from:  
https://www.kurnik.pl/tryktrak/zasady.phtml

---

## 📌 Game Goal

- Points **1–6** form the **home board of the red player**.  
- Points **19–24** form the **home board of the white player**.  
- The area to the right of the home board is the **yard**.

The objective is to move all of your pieces into your home board and then **bear them off into the yard**.  
The first player to bear off all their pieces wins.

---

## 🎲 Moving the Pieces

Pieces move in fixed directions:

- **White** moves **clockwise**: `1 → 12 → 13 → 24`  
- **Red** moves **counter-clockwise**: `24 → 13 → 12 → 1`

Movement rules:

- The two rolled dice determine the distances of two moves.
- Moves are performed one after another.
- If both dice show the same value (**a double**), the player makes **four moves** instead of two.
- Each move may be done with **any piece** (same or different).

A move may **end** only on a point that is:

- empty, or  
- occupied by any number of your own pieces, or  
- occupied by **exactly one** opponent piece (which is then hit).

A point with **two or more** opponent pieces is **blocked**.

Mandatory move usage:

- A player must use **each die value** if possible.  
- If only one die value can be used, the player must use that one.  
- If exactly one move is possible but it could be done using either die, the **higher** die must be used.  
- If no moves are available, the player **loses their turn**.

---

## ⚔️ Hitting (Capturing) Pieces

- Landing on a point with **exactly one** opponent piece hits it.  
- The hit piece is placed on the **bar** (the central vertical strip).

When a player has one or more pieces on the bar:

- They **must re-enter all of them** before making normal moves.

Re-entry rules:

- A piece is entered into the **opponent’s home board**.
- The entry point depends on the die value rolled.
- The target point must be:
  - empty, or  
  - occupied by your pieces, or  
  - containing exactly **one** opponent piece (which is hit)

**Example:**  
A red piece is on the bar. The player rolls **2** and **5**.  
They may re-enter the piece on:

- point **23** (24 - 1), or  
- point **20** (24 - 4),

as long as the point does **not** contain two or more white pieces.  
Any unused die value is then applied to another move.

If both re-entry points are blocked, the player **loses their turn**.

The same obligation applies: the player must use every die value if possible.

---

## 🏁 Bearing Off

Once a player has brought **all pieces** into their home board, they may begin **bearing off**.

A piece may be borne off if:

- It is on the point whose distance to the yard equals the die value.

**Example:**  
Red rolls **1** and **5** → they may remove pieces from points **1** and **5**.

If there is no piece on the point indicated by the die:

- The player must make a normal move with a piece on a **higher-numbered** point.
- If no such piece exists, the player may bear off from the **highest occupied point**.

**Example:**  
Red has pieces on points **5**, **2**, and **1**, and rolls **4** and **4**.

- No piece is on point 4.
- The piece on **5** must be moved to **2**.
- No further normal move is possible.
- Red may then bear off from the highest occupied point → **point 2**.

Bearing off is **not mandatory** — a player may choose to make a normal move instead.

---

## 🛠️ Technologies

- **C#**
- **WPF**
- Custom game logic based on authentic Backgammon rules
- Local game state simulation (no networking)

---

## 📸 Screenshots

<img width="1278" height="1088" alt="image" src="https://github.com/user-attachments/assets/b5149e0c-12a8-4183-8eba-0e904a1a201c" />
<img width="1280" height="1075" alt="image" src="https://github.com/user-attachments/assets/d4795a8c-dde1-44db-9a54-fd247f159d8d" />
<img width="1290" height="1087" alt="image" src="https://github.com/user-attachments/assets/c06c0379-19c6-4646-8c84-f588b39ec4ee" />

---

## ✔️ License

[LICENSE](./LICENSE.txt)

