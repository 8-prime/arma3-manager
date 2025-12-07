import { useState } from "react"
import { ThemeProvider } from "./components/theme-provider"
import { MainApp } from "./components/main-app"
import { InitializationScreen } from "./components/initialization-screen"

function App() {
  const [isInitialized, setIsInitialized] = useState(false)

  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <div className="min-h-screen bg-background">
        {!isInitialized && <InitializationScreen onComplete={() => setIsInitialized(true)} />}
        {isInitialized && <MainApp />}
      </div>
    </ThemeProvider>
  )
}

export default App
