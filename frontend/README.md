# FRIENDWISH — Frontend

Клиентская часть FRIENDWISH на Angular 22. Проект создан командой `ng new` (Angular CLI 22.2.2).

## Требования

- **Node.js** версии 22.22.3+, 24.15.0+ или 26+ (рекомендуется LTS 24). Вместе с Node.js устанавливается **npm**.
- **Git**.

Проверить установленные версии:

```bash
node -v
npm -v
```

Установить Node.js можно с [nodejs.org](https://nodejs.org/) или командой (Windows):

```bash
winget install OpenJS.NodeJS.LTS
```

Глобально устанавливать Angular CLI не нужно: он ставится вместе с зависимостями проекта и запускается через `npm` или `npx ng`.

## Установка после клонирования

```bash
git clone https://github.com/YOnlyLO/InthernetProgramming.git
cd InthernetProgramming/frontend
npm install
```

`npm install` скачивает все зависимости в папку `node_modules`. Ее нет в репозитории, поэтому команду нужно выполнить один раз после клонирования и повторять после изменения `package.json`.

## Запуск

```bash
npm start
```

После сборки приложение доступно по адресу [http://localhost:4200/](http://localhost:4200/). При изменении исходных файлов страница обновляется автоматически. Остановить сервер — `Ctrl+C`.

Если страница по адресу `localhost` не открывается, запустите сервер на IPv4-адресе и откройте [http://127.0.0.1:4200/](http://127.0.0.1:4200/):

```bash
npm start -- --host 127.0.0.1
```

## Сборка

```bash
npm run build
```

Готовые файлы появятся в папке `dist/friendwish`.

## Тесты

```bash
npm test
```

Модульные тесты запускаются через [Vitest](https://vitest.dev/).

## Создание компонентов

```bash
npx ng generate component component-name
```

Список всех доступных генераторов: `npx ng generate --help`. Подробнее — в [документации Angular CLI](https://angular.dev/tools/cli).
