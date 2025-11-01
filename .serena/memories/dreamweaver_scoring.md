# Dreamweaver Scoring - The Actual Simple Rule

## The Rule
**Only 1 or 2 points per choice:**

1. **If choice owner matches scene owner = 2 points** (aligned choice)
2. **If choice owner doesn't match scene owner = 1 point** (any other choice gets consolation point)

## Example from ghost.json

```json
{
  "id": "scene_003_first_reflection",
  "owner": "omega",
  "choice": [
    {
      "owner": "light",
      "text": "The seeker—searching for choice."
    },
    {
      "owner": "shadow",
      "text": "The keeper of secrets—holding what others fear."
    },
    {
      "owner": "ambition",
      "text": "The one who changes everything."
    }
  ]
}
```

When player picks the "light" choice:
- Scene owner: omega (narrator)
- Choice owner: light
- They don't match → **1 point to light**

## From nethack.json

```json
{
  "id": "scene_2_chamber_light_door",
  "owner": "light",
  "choice": [
    {
      "owner": "light",
      "text": "Speak the truth you've carried."
    },
    {
      "owner": "ambition",
      "text": "Question what 'truth' means here."
    }
  ]
}
```

When player picks the "light" choice:
- Scene owner: light
- Choice owner: light
- They match → **2 points to light**

When player picks the "ambition" choice:
- Scene owner: light
- Choice owner: ambition
- They don't match → **1 point to ambition**

## Implementation

No need for `Points` field in JSON at all. The logic is:

```csharp
int pointsToAward = (sceneOwner == choiceOwner) ? 2 : 1;
GameState.UpdateDreamweaverScore(choiceOwnerType, pointsToAward);
```

## What Must Never Break

1. Scene has an `owner` field
2. Choice has an `owner` field
3. Encounter has an `owner` field
4. Scoring compares them: match = 2, no match = 1
5. No exceptions, no special cases
6. extra story blocks should all have an `owner` field and calculations will always be the same scoring
