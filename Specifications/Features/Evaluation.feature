Feature: Word Evaluation
  As a player
  I want my guesses to be evaluated correctly
  So that the screen shows accurate feedback on letter positions

Background:
  Given I start a game with the target word is 'CRANE'

Scenario: Reject empty word on submit
  When I press the key 'ENTER'
  Then the game should trigger an invalid input state

Scenario: Reject incomplete word on submit
  When I type the word 'AGR'
  And I press the key 'ENTER'
  Then the game should trigger an invalid input state

Scenario: Reject incorrect word on submit
  Given the following words are invalid:
    | word  |
    | INVLD |
  When I type the word 'INVLD'
  And I press the key 'ENTER'
  Then the game should trigger an invalid input state

Scenario: Enter valid letters and see them on screen
  When I type the word 'AGREE'
  Then the following letters should have state 'Pending' on the game board:
    | letter |
    | A      |
    | G      |
    | R      |
    | E      |
    | E      |

Scenario: Incorrect guess updates tile colors
  Given the following words are valid:
    | word  |
    | AGREE |
  When I type the word 'AGREE'
  And I press the key 'ENTER'
  Then the following letters should have the specified states on the game board:
    | letter | state         |
    | A      | Elsewhere     |
    | G      | NoMoreMatches |
    | R      | Elsewhere     |
    | E      | NoMoreMatches |
    | E      | ExactMatch    |

Scenario: Incorrect guess updates keyboard visual
  Given the following words are valid:
    | word  |
    | AGREE |
  When I type the word 'AGREE'
  And I press the key 'ENTER'
  Then the following letters should have the specified states on the on screen keyboard:
    | letter | state         |
    | A      | Elsewhere     |
    | G      | NoMoreMatches |
    | R      | Elsewhere     |
    | E      | ExactMatch    |

Scenario: Incorrect guess updates keyboard visual on subsequent guesses
  Given the following words are valid:
    | word  |
    | AGREE |
    | BRAIN |
  When I type the word 'AGREE'
  And I press the key 'ENTER'
  And I type the word 'BRAIN'
  And I press the key 'ENTER'
  Then the following letters should have the specified states on the on screen keyboard:
    | letter | state         |
    | A      | ExactMatch    |
    | G      | NoMoreMatches |
    | R      | ExactMatch    |
    | E      | ExactMatch    |
    | B      | NoMoreMatches |
    | I      | NoMoreMatches |
    | N      | Elsewhere     |

Scenario: Lose the game and see game over screen
  Given the following words are valid:
    | word  |
    | AGREE |
  When I repeatedly submit the input 'AGREE'
  Then I should see the game over screen

Scenario: Win the game and see game over screen
  When I type the word 'CRANE'
  And I press the key 'ENTER'
  Then I should see the game over screen