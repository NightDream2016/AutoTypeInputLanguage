# Auto Type Input Language
A [KeePass v2.x](https://keepass.info/) plugin to auto switch input language when trigger auto-type.

## Purpose
In some multi-language / non-english language Windows environment (like Chinese / Japanese user), when using the global auto-type hot key, it will lead to incorrect input when input language is not english in foreground window.

## Usage
1. add the English-Unites States (or other language could input your password correctly) to input language list in the Setting / Time & Language page of Windows.
2. Set your preferred input language in **Tools >> Auto Type Input Language ** .
3. Trigger the auto-type flow by hotkey / otherway, you will noticed that no matter what input language you use in your foreground window, it will auto-switch to you choosed language.

## Installation
Just download the AutoTypeInputLanguage.plgx file (see files above or click here), copy it to the KeePass application plugin directory and restart KeePass.

## Environment
Tested OK on Windows 10 2004.

## 目的
- 為了解決個人使用KeePass時經常遇到視窗採用的輸入語言並非英數而在自動輸入密碼時發生錯誤的問題(最常見的就是用中文輸入法導致密碼Key錯)，
- 網路上有設定輸入法快捷鍵然後再讓KeePass的Auto-Type序列去trigger的做法，但依個人經驗來說不太可靠。
  - 很多時候無法順利切換成功，結果還是要手動切回英數再按Ctrl+A，實在太麻煩。
- 最後乾脆自己用C#寫一個Plugin一勞永逸，就是本Plugin了。

## 使用方法
- 安裝此Plugin以後在KeePass的工具底下會多出一欄，其為目前使用者Windows的語系列表。
- 選擇要使用的預設輸入法。
  - 系統上有 **英文(美國)** 的情況，Plugin會自動指定它為預設輸入法。若是沒有的話則會拿輸入法列表當中的第一個。
  - 可自由切換成你想要的輸入語言 (如果你真的想要用英數以外的語系做為密碼的話XD)
- 之後每次Trigger KeePass的自動輸入密碼時，系統就會自動切到你選擇的輸入法了。

## 測試版本
- Windows 10 2004版已通過測試可使用。
