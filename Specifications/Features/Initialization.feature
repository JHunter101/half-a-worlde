Feature: Initialization
  As a player
  I want to configure and start a game
  So that I can play with my preferred rules

Background: 
    Given I am on the home screen

Scenario: Main Menu
    Then I should see the main menu

Scenario: Initial keyboard no hints
    Given No hints are available
    When I click the 'start game' button
	Then all letters should have state 'Pending' on the on screen keyboard

Scenario: Initial hints
  Given the following words are valid hints:
    | word  |
    | BLISS |
    | FIGHT |
  And I start a game with the target word is 'CRANE'
  Then the following words should be visible on the game board:
    | word  |
    | BLISS |
    | FIGHT |


Scenario Outline: Start game with custom settings
    When I set the word length to <word length>
    And I set the max attempts to <max attempts>
    And I click the 'start game' button
    Then the game board should be visible
    And I should see <letter tiles> letter tiles on the screen
    Examples:
        | word length | max attempts | letter tiles |
        | 5           | 6            | 30           |
        | 4           | 6            | 24           |
        | 6           | 8            | 48           |
