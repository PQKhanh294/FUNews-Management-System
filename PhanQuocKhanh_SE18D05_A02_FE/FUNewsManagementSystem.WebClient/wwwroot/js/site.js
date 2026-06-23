(function () {
    const tokenKey = "funews.jwt";
    const userKey = "funews.user";

    window.pick = function (obj, ...names) {
        for (const name of names) {
            if (obj && obj[name] !== undefined && obj[name] !== null) return obj[name];
        }
        return undefined;
    };

    window.getAuthToken = function () {
        return sessionStorage.getItem(tokenKey);
    };

    window.getCurrentUser = function () {
        const raw = sessionStorage.getItem(userKey);
        return raw ? JSON.parse(raw) : null;
    };

    window.setAuth = function (response) {
        sessionStorage.setItem(tokenKey, response.accessToken || response.AccessToken);
        sessionStorage.setItem(userKey, JSON.stringify({
            accountId: response.accountId || response.AccountId,
            email: response.email || response.Email,
            role: response.role || response.Role,
            avatarUrl: response.avatarUrl || response.AvatarUrl
        }));
    };

    window.clearAuth = function () {
        sessionStorage.removeItem(tokenKey);
        sessionStorage.removeItem(userKey);
    };

    window.callApi = function (method, url, data, onSuccess, onError) {
        const headers = {};
        const token = getAuthToken();
        if (token) headers.Authorization = "Bearer " + token;

        $.ajax({
            method: method,
            url: window.FUNEWS_API_BASE + url,
            data: data ? JSON.stringify(data) : undefined,
            contentType: data ? "application/json" : undefined,
            headers: headers,
            success: onSuccess,
            error: function (xhr) {
                const isConnectionError = xhr.status === 0;
                const message = isConnectionError
                    ? `Cannot connect to Web API at ${window.FUNEWS_API_BASE}. Start the BE project or check ApiBaseUrl.`
                    : xhr.responseJSON?.error || xhr.responseJSON?.message || xhr.responseText || `Request failed (${xhr.status})`;
                if (onError) onError(message, xhr);
                else showToast(message, "danger");
            }
        });
    };

    window.showToast = function (message, type) {
        const id = "toast-" + Date.now();
        const html = `<div id="${id}" class="toast align-items-center text-bg-${type || "primary"} border-0" role="alert">
            <div class="d-flex"><div class="toast-body">${message}</div><button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button></div>
        </div>`;
        $("#toastHost").append(html);
        const toast = new bootstrap.Toast(document.getElementById(id), { delay: 3500 });
        toast.show();
    };

    window.confirmAction = function (message, onConfirm) {
        const id = "confirm-" + Date.now();
        const html = `<div class="modal fade" id="${id}" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered"><div class="modal-content">
                <div class="modal-header"><h5 class="modal-title">Confirm</h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div>
                <div class="modal-body">${escapeHtml(message)}</div>
                <div class="modal-footer"><button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancel</button><button type="button" class="btn btn-danger btn-confirm">Delete</button></div>
            </div></div>
        </div>`;
        $("body").append(html);
        const element = document.getElementById(id);
        const modal = new bootstrap.Modal(element);
        $(element).on("click", ".btn-confirm", function () {
            modal.hide();
            onConfirm();
        });
        element.addEventListener("hidden.bs.modal", function () {
            element.remove();
        });
        modal.show();
    };

    window.approvalName = function (value) {
        return ["Draft", "PendingApproval", "Approved", "Rejected"][Number(value)] || value;
    };

    window.roleName = function (value) {
        return Number(value) === 1 ? "Staff" : Number(value) === 2 ? "Lecturer" : "Admin";
    };

    window.escapeHtml = function (value) {
        return $("<div>").text(value || "").html();
    };

    window.sanitizeRichHtml = function (html) {
        const template = document.createElement("template");
        template.innerHTML = html || "";

        const allowedTags = new Set(["P", "BR", "STRONG", "B", "EM", "I", "U", "A", "OL", "UL", "LI", "H2", "H3", "BLOCKQUOTE", "SPAN"]);
        const allowedAttributes = new Set(["href", "target", "rel", "class"]);

        template.content.querySelectorAll("*").forEach(element => {
            if (!allowedTags.has(element.tagName)) {
                element.replaceWith(...element.childNodes);
                return;
            }

            [...element.attributes].forEach(attribute => {
                const name = attribute.name.toLowerCase();
                const value = attribute.value || "";
                if (!allowedAttributes.has(name) || name.startsWith("on") || value.toLowerCase().startsWith("javascript:")) {
                    element.removeAttribute(attribute.name);
                }
            });

            if (element.tagName === "A") {
                element.setAttribute("target", "_blank");
                element.setAttribute("rel", "noopener noreferrer");
            }
        });

        return template.innerHTML;
    };

    function renderRoleMenu() {
        const user = getCurrentUser();
        $(".auth-only").toggleClass("d-none", !user);
        $(".guest-only").toggleClass("d-none", !!user);
        $(".role-admin").toggleClass("d-none", user?.role !== "Admin");
        $(".role-staff").toggleClass("d-none", user?.role !== "Staff");
        $(".role-admin-staff").toggleClass("d-none", user?.role !== "Admin" && user?.role !== "Staff");

        // Populate user display name, initials & role badge in navbar
        if (user) {
            const displayName = user.email ? user.email.split("@")[0] : "User";
            const initial = displayName.charAt(0).toUpperCase();
            $("#userDisplayName").text(displayName);
            $("#userInitial").text(initial);
            // Role badge (Admin / Staff)
            const roleLabel = user.role || "";
            const badge = $("#navRoleBadge");
            if (badge.length && roleLabel) badge.text(roleLabel).removeClass("d-none");
        }
    }


    function renderCategoryNavItem(node, depth) {
        const name = escapeHtml(pick(node, "categoryName", "CategoryName"));
        const id   = pick(node, "categoryID", "CategoryID");
        const children = pick(node, "children", "Children") || [];
        const selected = new URLSearchParams(window.location.search).get("category") === String(id);
        const hasChildren = children.length > 0;
        const childHtml = hasChildren
            ? `<ul class="category-menu-children" role="group">${children.map(child => renderCategoryNavItem(child, depth + 1)).join("")}</ul>`
            : "";
        const toggle = hasChildren
            ? `<button class="category-submenu-toggle" type="button" aria-expanded="false" aria-label="Open ${name} subcategories">
                    <svg width="10" height="10" viewBox="0 0 16 16" fill="currentColor" aria-hidden="true"><path d="m6 3.5 4.5 4.5L6 12.5z"/></svg>
               </button>`
            : "";

        return `<li class="category-menu-item${hasChildren ? " has-children" : ""}" role="none">
            <div class="category-menu-row" style="--category-depth:${depth}">
                <a class="nav-dropdown-item${selected ? " active" : ""}" href="/?category=${id}" role="menuitem"${selected ? ' aria-current="page"' : ""}>${name}</a>
                ${toggle}
            </div>
            ${childHtml}
        </li>`;
    }

    function renderCategoryTree(nodes) {
        return `<ul class="category-menu-root" role="menu">${nodes.map(node => renderCategoryNavItem(node, 0)).join("")}</ul>`;
    }

    function usesHoverCategoryMenu() {
        return window.matchMedia("(hover: hover) and (min-width: 769px)").matches;
    }

    function closeCategoryBranch(item) {
        item.removeClass("open");
        item.find(".category-menu-item.open").removeClass("open");
        item.find(".category-submenu-toggle").attr("aria-expanded", "false");
    }

    function loadCategoryMenu() {
        callApi("GET", "/odata/Categories/Tree/Public", null, function (items) {
            const nodes = items.value || items || [];
            const container = $("#categoryDropdown");
            if (!container.length) return;
            if (!nodes.length) {
                container.html('<span class="nav-dropdown-item" style="opacity:.5">No categories</span>');
                return;
            }
            container.html(renderCategoryTree(nodes));
        }, function () {
            // silently fail — category menu is optional
        });
    }

    function renderActiveNav() {
        const currentPath = window.location.pathname.toLowerCase();
        $(".app-navbar .nav-link").each(function () {
            const href = ($(this).attr("href") || "").toLowerCase();
            if (href && href !== "#" && (currentPath === href || (href !== "/" && currentPath.startsWith(href)))) {
                $(this).addClass("active");
            }
        });
        if (currentPath === "/") {
            $(".app-navbar .nav-link[href='/']").addClass("active");
        }
    }

    $(function () {
        renderRoleMenu();
        loadCategoryMenu();
        renderActiveNav();
        $(document).on("click", "#btnLogout", function () {
            clearAuth();
            window.location.href = "/";
        });

        $(document).on("click", ".category-submenu-toggle", function (event) {
            event.preventDefault();
            event.stopPropagation();
            const item = $(this).closest(".category-menu-item");

            // Desktop submenus are controlled exclusively by hover. Keeping an
            // .open class after a click would make old and new fly-outs overlap.
            if (usesHoverCategoryMenu()) {
                $("#categoryMenu .category-menu-item.open").each(function () {
                    closeCategoryBranch($(this));
                });
                return;
            }

            item.siblings(".category-menu-item.open").each(function () {
                closeCategoryBranch($(this));
            });
            const isOpen = item.toggleClass("open").hasClass("open");
            $(this).attr("aria-expanded", String(isOpen));
            if (!isOpen) {
                closeCategoryBranch(item);
            }
        });

        $(document).on("mouseenter", ".category-menu-item.has-children", function () {
            if (!usesHoverCategoryMenu()) return;
            $(this).siblings(".category-menu-item.open").each(function () {
                closeCategoryBranch($(this));
            });
            $(this).children(".category-menu-row").find(".category-submenu-toggle").attr("aria-expanded", "true");
        });

        $(document).on("mouseleave", ".category-menu-item.has-children", function () {
            if (!usesHoverCategoryMenu()) return;
            $(this).children(".category-menu-row").find(".category-submenu-toggle").attr("aria-expanded", "false");
        });

        $(window).on("resize", function () {
            if (!usesHoverCategoryMenu()) return;
            $("#categoryMenu .category-menu-item.open").each(function () {
                closeCategoryBranch($(this));
            });
        });

        $(document).on("keydown", ".category-menu-row a, .category-submenu-toggle", function (event) {
            const item = $(this).closest(".category-menu-item");
            if (event.key === "ArrowRight" && item.hasClass("has-children")) {
                item.addClass("open").children(".category-menu-row").find(".category-submenu-toggle").attr("aria-expanded", "true");
                item.children(".category-menu-children").find("a").first().trigger("focus");
                event.preventDefault();
            } else if (event.key === "ArrowLeft") {
                const parent = item.parent().closest(".category-menu-item");
                if (parent.length) {
                    parent.removeClass("open").children(".category-menu-row").find(".category-submenu-toggle").attr("aria-expanded", "false");
                    parent.children(".category-menu-row").find("a").first().trigger("focus");
                    event.preventDefault();
                }
            } else if (event.key === "Escape") {
                $("#categoryMenu").removeClass("open");
                $("#categoryMenu .category-menu-item").removeClass("open");
                $("#categoryMenu .category-submenu-toggle").attr("aria-expanded", "false");
                $("#categoryMenu .nav-dropdown-toggle").trigger("focus");
                event.preventDefault();
            }
        });
    });
})();

