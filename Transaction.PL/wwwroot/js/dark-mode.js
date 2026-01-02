// ============================================
// Dark Mode / Light Mode Toggle
// ============================================

(function () {
    'use strict';

    // Get theme from localStorage or default to light
    const getTheme = () => {
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme) {
            return savedTheme;
        }
        // Check system preference
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            return 'dark';
        }
        return 'light';
    };

    // Apply theme to document
    const applyTheme = (theme) => {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);

        // Update toggle button icon
        updateToggleIcon(theme);

        // Apply theme to selects
        applyThemeToSelects(theme);
    };

    // Update toggle icon
    const updateToggleIcon = (theme) => {
        const icon = document.querySelector('.theme-toggle-slider i');
        if (icon) {
            if (theme === 'dark') {
                icon.className = 'fas fa-moon';
            } else {
                icon.className = 'fas fa-sun';
            }
        }
    };

    // Toggle theme
    const toggleTheme = () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        applyTheme(newTheme);
    };

    // Apply theme to all select elements and their options
    const applyThemeToSelects = (theme) => {
        const allSelects = document.querySelectorAll('select');

        allSelects.forEach(select => {
            if (theme === 'dark') {
                // Apply dark theme to select
                select.style.backgroundColor = '#0d0d1a';
                select.style.color = '#ffffff';
                select.style.borderColor = '#3d4663';

                // Apply dark theme to all options
                const options = select.querySelectorAll('option');
                options.forEach(option => {
                    option.style.backgroundColor = '#000000';
                    option.style.color = '#ffffff';

                    // Special styling for placeholder options
                    if (option.value === '' || option.value === null) {
                        option.style.color = '#999999';
                        option.style.fontStyle = 'italic';
                    }
                });
            } else {
                // Reset to light theme
                select.style.backgroundColor = '';
                select.style.color = '';
                select.style.borderColor = '';

                const options = select.querySelectorAll('option');
                options.forEach(option => {
                    option.style.backgroundColor = '';
                    option.style.color = '';
                    option.style.fontStyle = '';
                });
            }
        });
    };

    // Initialize theme on page load
    document.addEventListener('DOMContentLoaded', function () {
        // Apply saved theme
        const theme = getTheme();
        applyTheme(theme);

        // Create theme toggle button
        createThemeToggle();

        // Apply theme to all select options
        applyThemeToSelects(theme);

        // Listen for system theme changes
        if (window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
                const newTheme = e.matches ? 'dark' : 'light';
                applyTheme(newTheme);
                applyThemeToSelects(newTheme);
            });
        }
    });

    // Create theme toggle button in navbar
    const createThemeToggle = () => {
        const navbarNav = document.querySelector('.navbar-nav.ms-auto');
        if (!navbarNav) return;

        // Create toggle container
        const toggleContainer = document.createElement('li');
        toggleContainer.className = 'nav-item theme-toggle-container';

        // Create toggle button
        const toggleButton = document.createElement('div');
        toggleButton.className = 'theme-toggle';
        toggleButton.setAttribute('role', 'button');
        toggleButton.setAttribute('aria-label', 'Toggle dark mode');
        toggleButton.setAttribute('tabindex', '0');

        // Create slider
        const slider = document.createElement('div');
        slider.className = 'theme-toggle-slider';

        const icon = document.createElement('i');
        icon.className = 'fas fa-sun';

        slider.appendChild(icon);
        toggleButton.appendChild(slider);
        toggleContainer.appendChild(toggleButton);

        // Add click event
        toggleButton.addEventListener('click', toggleTheme);

        // Add keyboard support
        toggleButton.addEventListener('keypress', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                toggleTheme();
            }
        });

        // Insert before profile dropdown or at the end
        const profileDropdown = navbarNav.querySelector('.profile-dropdown');
        if (profileDropdown) {
            navbarNav.insertBefore(toggleContainer, profileDropdown);
        } else {
            navbarNav.appendChild(toggleContainer);
        }

        // Update icon based on current theme
        const currentTheme = getTheme();
        updateToggleIcon(currentTheme);
    };

    // Export functions if needed
    window.themeManager = {
        toggle: toggleTheme,
        apply: applyTheme,
        get: getTheme
    };
})();