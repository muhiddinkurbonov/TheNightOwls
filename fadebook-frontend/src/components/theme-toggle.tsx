'use client';

import * as React from 'react';
import { Moon, Sun } from 'lucide-react';
import { useTheme } from 'next-themes';
import { themeApi, type ThemeValue } from '@/lib/api/theme';

import { Button } from '@/components/ui/button';

export function ThemeToggle() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = React.useState(false);

  // Sync initial theme from backend cookie
  React.useEffect(() => {
    setMounted(true);
    (async () => {
      try {
        const t = await themeApi.get();
        setTheme(t);
      } catch {
        // ignore
      }
    })();
  }, [setTheme]);

  const applyTheme = async (value: ThemeValue) => {
    // Optimistically update UI
    setTheme(value);
    try {
      await themeApi.set(value);
    } catch {
      // swallow; user keeps chosen theme even if persistence fails
    }
  };

  const toggleTheme = () => {
    const newTheme = theme === 'dark' ? 'light' : 'dark';
    applyTheme(newTheme);
  };

  // Avoid hydration mismatch
  if (!mounted) {
    return (
      <Button variant="outline" size="icon" disabled>
        <Sun className="h-[1.2rem] w-[1.2rem]" />
        <span className="sr-only">Toggle theme</span>
      </Button>
    );
  }

  return (
    <Button variant="outline" size="icon" onClick={toggleTheme}>
      <Sun className="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
      <Moon className="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
      <span className="sr-only">Toggle theme</span>
    </Button>
  );
}
