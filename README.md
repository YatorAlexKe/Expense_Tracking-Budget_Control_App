# FinanceTracker

> **Track your money. Own your future.**

FinanceTracker is a secure, privacy-first personal finance web application that helps individuals take full control of their financial health. Whether you want to track daily expenses, set monthly budgets, monitor your crypto portfolio or get a clear picture of your net worth — FinanceTracker brings it all together in one clean, intuitive interface.

---

## Team Members

| Name | Student Number | Role |
|------|---------------|------|
| Alex Yator | A00335392 | Backend Developer / Frontend Developer |
| Cherylpreet Bansal | A00336894 | Backend Developer / Frontend Developer |
| Noble Udecchukwu | A00335177 | Backend Developer / Frontend Developer |
| Audrey Goudi | A00335177 | Backend Developer / Frontend Developer |

---

## Project Overview

FinanceTracker is a full-stack personal finance application built with a modern **ASP.NET Core Web API** backend and a responsive **HTML/CSS/JavaScript** frontend. It was developed as a **Cambrian College Software Engineering Capstone Project** and is designed to be simple enough for everyday users while being powerful enough to give real financial insight.

### Who is it for?
- **Individuals** who want to understand where their money goes each month
- **Budget-conscious users** who need to set spending limits and get warned before exceeding them
- **Investors** who want to track their crypto portfolio alongside regular finances
- **Anyone** who wants a private, ad-free alternative to cloud-based finance apps

### What problem does it solve?
Most people have no clear picture of their financial health. They don't know how much they spend on groceries vs dining out, whether they're on track with their budget, or what their total net worth looks like across all their accounts. FinanceTracker solves this by putting all that information in one place — clearly, securely and without ads.

---
## ✨ Features

| # | Feature | Description | Primary | Secondary |
|---|---------|-------------|---------|-----------|
| 1 | **Expense Tracking** | Log and categorise every transaction with default or custom categories. Filter by date range and category. Export to CSV. | Alex Yator | Cherylpreet Bansal |
| 2 | **Monthly Budgets & Alerts** | Set monthly spending limits per category. Real-time utilization tracking with Ok / Warning / Exceeded status badges and progress bars. | Cherylpreet Bansal | Noble Udecchukwu |
| 3 | **Income Tracking** | Log income by type (Salary, Freelance, Investment, etc.). Custom income types supported. Income deposited into accounts updates Net Worth automatically. | Noble Udecchukwu | Audrey Goudi |
| 4 | **Net Worth Dashboard** | Aggregates bank accounts, crypto portfolio and income into a single net worth figure. Displays monthly cash flow and savings rate. | Audrey Goudi | Alex Yator |
| 5 | **Crypto Portfolio** | Manually track digital assets (BTC, ETH, SOL, etc.) with average buy price, current mock price, total value and profit/loss calculation. | Alex Yator | Noble Udecchukwu |
| 6 | **Password Reset via Email** | Secure forgot-password flow using SendGrid. One-time reset links expire after 30 minutes. Password strength enforced on register and reset. | Cherylpreet Bansal | Audrey Goudi |

---

## 🔄 Application Flow

### Step 1 — Register & Login
The user creates an account with a valid email and a strong password (min 8 characters, uppercase, lowercase and a number). On successful registration the user is automatically logged in and a JWT token is issued for the session.

```
┌─────────────────────────────────────┐
│         FinanceTracker              │
│   Track your money, own your future │
├─────────────────────────────────────┤
│  [ Sign In ]     [ Register ]       │
│                                     │
│  EMAIL                              │
│  ┌─────────────────────────────┐    │
│  │ you@example.com             │    │
│  └─────────────────────────────┘    │
│  PASSWORD                           │
│  ┌─────────────────────────────┐    │
│  │ ••••••••               👁   │    │
│  └─────────────────────────────┘    │
│                                     │
│  ┌─────────────────────────────┐    │
│  │         Sign In             │    │
│  └─────────────────────────────┘    │
│                                     │
│      Forgot your password?          │
│         Try Demo Mode               │
└─────────────────────────────────────┘
```

### Step 2 — Set Up Categories & Budgets
The user navigates to **Manage** to review default expense categories (Rent, Groceries, Transport, etc.) and add custom ones. They then go to **Budgets** to set monthly spending limits per category.

```
┌─────────────────────────────────────┐
│  Manage                             │
├─────────────────────────────────────┤
│  EXPENSE CATEGORIES    [+ New]      │
│  ┌──────────────┐ ┌──────────────┐  │
│  │ Rent Default │ │Groceries Def │  │
│  └──────────────┘ └──────────────┘  │
│  ┌──────────────┐ ┌──────────────┐  │
│  │Transport Def │ │ Fuel  Default│  │
│  └──────────────┘ └──────────────┘  │
│                                     │
│  INCOME TYPES          [+ New]      │
│  ┌──────────────┐ ┌──────────────┐  │
│  │ Salary  Def  │ │Freelance Def │  │
│  └──────────────┘ └──────────────┘  │
└─────────────────────────────────────┘
```
### Step 3 — Log Income & Expenses
The user logs their income specifying the source, type and which bank account to deposit into — automatically updating their Net Worth. They then log daily expenses assigning each to a category.

```
┌─────────────────────────────────────┐
│  Add Income                         │
├─────────────────────────────────────┤
│  AMOUNT ($)                         │
│  ┌─────────────────────────────┐    │
│  │ 5800.00                     │    │
│  └─────────────────────────────┘    │
│  SOURCE                             │
│  ┌─────────────────────────────┐    │
│  │ Monthly Salary              │    │
│  └─────────────────────────────┘    │
│  TYPE          DEPOSIT INTO         │
│  ┌──────────┐  ┌───────────────┐    │
│  │ Salary ▼ │  │Chase Checking │    │
│  └──────────┘  └───────────────┘    │
│                                     │
│  [Cancel]          [Save]           │
└─────────────────────────────────────┘
```
### Step 4 — Monitor Net Worth & Dashboard
The user visits the **Net Worth** page to see total assets, monthly income vs expenses and savings rate. The **Dashboard** shows budget utilization per category with progress bars and status badges.

```
┌─────────────────────────────────────┐
│  Net Worth                          │
├─────────────────────────────────────┤
│        TOTAL NET WORTH              │
│          $105,431.30                │
│                                     │
│  Total Assets    $105,431.30        │
│  Monthly Income  $7,320.00          │
│  Monthly Expenses $654.38           │
│                                     │
│  ASSET BREAKDOWN                    │
│  Fidelity Brokerage   $54,780.50    │
│  Marcus Savings       $38,200.00    │
│  Chase Checking       $12,450.80    │
│                                     │
│  CASH FLOW                          │
│  Income      $7,320.00              │
│  Expenses   -$654.38                │
│  Net Flow   +$6,665.62              │
│  Savings Rate    91%                │
└─────────────────────────────────────┘
```
