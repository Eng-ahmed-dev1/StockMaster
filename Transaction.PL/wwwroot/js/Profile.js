// Profile Management Script
class ProfileManager {
    constructor() {
        this.dropdown = document.getElementById('profileDropdown');
        this.toggle = document.getElementById('profileToggle');
        this.isOpen = false;

        this.init();
    }

    init() {
        if (this.toggle && this.dropdown) {
            this.toggle.addEventListener('click', (e) => this.toggleDropdown(e));
            this.setupClickOutside();
            this.loadUserData();
        }
    }

    toggleDropdown(e) {
        e.preventDefault();
        e.stopPropagation();
        this.isOpen = !this.isOpen;
        this.dropdown.classList.toggle('open', this.isOpen);
    }

    setupClickOutside() {
        document.addEventListener('click', (e) => {
            if (!this.dropdown.contains(e.target)) {
                this.closeDropdown();
            }
        });

        // Close on escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.closeDropdown();
            }
        });
    }

    closeDropdown() {
        this.isOpen = false;
        this.dropdown.classList.remove('open');
    }

    async loadUserData() {
        try {
            // Check if user is authenticated
            const isAuthenticated = document.body.getAttribute('data-user-authenticated') === 'true';

            if (isAuthenticated) {
                const response = await fetch('/api/user/profile');
                if (response.ok) {
                    const data = await response.json();
                    this.updateUI(data);
                }
            }
        } catch (error) {
            console.error('Failed to load user data:', error);
        }
    }

    updateUI(userData) {
        // Update avatar
        const avatar = document.getElementById('userAvatar');
        const largeAvatar = document.getElementById('profileAvatarLarge');
        const name = document.getElementById('userName');
        const profileName = document.getElementById('profileName');
        const profileEmail = document.getElementById('profileEmail');
        const profileRoles = document.getElementById('profileRoles');

        if (avatar && userData.initials) {
            avatar.textContent = userData.initials;
            avatar.style.background = this.getAvatarColor(userData.initials);
        }

        if (largeAvatar && userData.initials) {
            largeAvatar.textContent = userData.initials;
            largeAvatar.style.background = this.getAvatarColor(userData.initials);
        }

        if (name && userData.firstName) {
            name.textContent = userData.firstName;
        }

        if (profileName && userData.fullName) {
            profileName.textContent = userData.fullName;
        }

        if (profileEmail && userData.email) {
            profileEmail.textContent = userData.email;
        }

        if (profileRoles && userData.roles) {
            profileRoles.innerHTML = '';
            userData.roles.forEach(role => {
                const badge = document.createElement('span');
                badge.className = `badge ${this.getRoleClass(role)}`;
                badge.textContent = role;
                profileRoles.appendChild(badge);
            });
        }

        // Update notifications count
        if (userData.notificationCount > 0) {
            this.updateNotificationBadge(userData.notificationCount);
        }
    }

    getAvatarColor(initials) {
        const colors = [
            'linear-gradient(45deg, #3498db, #2980b9)',
            'linear-gradient(45deg, #e74c3c, #c0392b)',
            'linear-gradient(45deg, #2ecc71, #27ae60)',
            'linear-gradient(45deg, #9b59b6, #8e44ad)',
            'linear-gradient(45deg, #f39c12, #d35400)'
        ];

        const index = initials.charCodeAt(0) % colors.length;
        return colors[index];
    }

    getRoleClass(role) {
        switch (role.toLowerCase()) {
            case 'admin': return 'bg-danger';
            case 'manager': return 'bg-primary';
            case 'moderator': return 'bg-warning';
            default: return 'bg-success';
        }
    }

    updateNotificationBadge(count) {
        const notificationLink = document.querySelector('.profile-links a[href*="notifications"]');
        if (notificationLink) {
            let badge = notificationLink.querySelector('.badge');
            if (!badge) {
                badge = document.createElement('span');
                badge.className = 'badge bg-danger ms-auto';
                notificationLink.appendChild(badge);
            }
            badge.textContent = count;
        }
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    const profileManager = new ProfileManager();

    // Add keyboard navigation
    document.addEventListener('keydown', (e) => {
        if (e.key === 'p' && (e.ctrlKey || e.metaKey)) {
            e.preventDefault();
            profileManager.toggleDropdown(new Event('click'));
        }
    });
});