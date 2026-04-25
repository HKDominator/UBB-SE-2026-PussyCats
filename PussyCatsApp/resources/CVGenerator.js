/**
 * ─────────────────────────────────────────────────────────────────
 * Called by PdfExportService via ExecuteScriptAsync after the
 * template page has fully loaded in WebView2.
 *
 * Strategy: inject data directly into the live DOM rather than
 * rewriting the document. This is reliable with WebView2 +
 * PrintToPdfAsync because the DOM is already stable when the
 * script runs.
 * ─────────────────────────────────────────────────────────────────
 */

const CVGenerator = (() => {

    const SKILL_GROUPS = {
        Languages: ['JavaScript', 'TypeScript', 'Python', 'Java', 'C#', 'C++', 'Go', 'Rust',
            'PHP', 'Ruby', 'Swift', 'Kotlin', 'Scala', 'R', 'Julia',
            'HTML', 'CSS', 'SCSS'],
        Frameworks: [
            'React', 'Angular', 'Vue.js', 'Next.js', 'Svelte',
            'Node.js', 'Spring Boot', 'ASP.NET Core', 'Django', 'Flask', 'FastAPI'
        ],
        'DevOps / Cloud': [
            'Docker', 'Kubernetes', 'Docker Swarm', 'OpenShift', 'Podman',
            'Git', 'GitHub', 'CI/CD', 'Jenkins', 'GitHub Actions',
            'AWS', 'Azure', 'Google Cloud',
            'Terraform', 'Ansible', 'Pulumi',
            'Linux', 'Bash',
            'Prometheus', 'Grafana', 'Datadog'
        ],
        Databases: [
            'SQL Server', 'PostgreSQL', 'MySQL', 'MongoDB', 'Redis', 'Oracle', 'SQL', 'BigQuery'
        ],
        'Data & AI': [
            'ML', 'Deep Learning',
            'TensorFlow', 'PyTorch', 'scikit-learn', 'Keras',
            'Pandas', 'NumPy',
            'Apache Spark', 'MLflow', 'Hugging Face',
            'OpenCV', 'NLTK', 'spaCy',
            'Descriptive Statistics', 'Regression', 'Hypothesis Testing',
            'Linear Algebra', 'Calculus', 'Probability', 'Statistics'
        ],
        Design: [
            'Figma', 'Adobe XD', 'Zeplin', 'Sketch', 'InVision', 'Marvel', 'Axure',
            'UI/UX', 'Wireframing', 'Prototyping', 'Figma Prototyping',
            'Typography', 'Color Theory', 'Grid Systems',
            'Storybook'
        ],
        'Soft Skills': [
            'Teamwork', 'Communication', 'Problem Solving',
            'Leadership', 'Time Management'
        ],
        Other: [],
    };

    // ── Helpers ───────────────────────────────────────────────────

    function esc(str) {
        if (!str) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    function hasValue(v) {
        if (v === null || v === undefined) return false;
        if (Array.isArray(v)) return v.length > 0;
        if (typeof v === 'string') return v.trim() !== '';
        if (typeof v === 'number') return v !== 0;
        return Boolean(v);
    }

    function formatDate(dateValue) {
        if (!dateValue) return '';
        // extract just the date part
        if (typeof dateValue === 'string') {
            return dateValue.split('T')[0];
        }
        // i it's a Date object, format it
        if (dateValue instanceof Date) {
            return dateValue.toISOString().split('T')[0];
        }
        return String(dateValue);
    }

    // ── Renderers ─────────────────────────────────────────────────

    function renderWorkExperience(entries = []) {
        return entries.map(w => `
        <div class="work-entry">
            <div class="work-line">
                <span class="work-role">${esc(w.jobTitle)}</span>
                <span class="work-company">${esc(w.company)}</span>
                <span class="work-divider"></span>
                <span class="work-period">${formatDate(w.startDate)} – ${w.currentlyWorking || !w.endDate ? 'Present' : formatDate(w.endDate)}</span>
            </div>
            ${w.description ? `<p class="work-description">${esc(w.description)}</p>` : ''}
        </div>`).join('');
    }

    function renderProjects(entries = []) {
        return entries.map(p => `
        <div class="project-entry-custom">
            <div class="project-line">
                <span class="project-title">${esc(p.name)}</span>
                <span class="project-divider"></span>
                ${p.url ? `<span class="project-period"><a href="${esc(p.url)}" style="color:#2d6a9f;font-size:7.5pt">${esc(p.url)}</a></span>` : ''}
            </div>
            ${p.technologies?.length ? `<div class="project-tech">${p.technologies.map(esc).join(' · ')}</div>` : ''}
            ${p.description ? `<p class="project-description">${esc(p.description)}</p>` : ''}
        </div>`).join('');
    }

    function renderExtraCurricular(entries = []) {
        return entries.map(a => `
        <div class="extra-entry">
            <div class="extra-title">${esc(a.activityName)}</div>
            <div class="extra-meta">${[a.organization, a.role, a.period].filter(Boolean).map(esc).join(' · ')}</div>
            ${a.description ? `<div class="extra-desc">${esc(a.description)}</div>` : ''}
        </div>`).join('');
    }

    function renderSkills(skills = []) {
        const buckets = {};
        skills.map(UnTrimmedSkill => UnTrimmedSkill.trim()).forEach(skill => {
            let placed = false;
            for (const [group, keywords] of Object.entries(SKILL_GROUPS)) {
                if (group === 'Other') continue;
                if (keywords.some(SkillToBeMatched => SkillToBeMatched.toLowerCase() === skill.toLowerCase())) {
                    buckets[group] = buckets[group] || [];
                    buckets[group].push(skill);
                    placed = true;
                    break;
                }
            }
            if (!placed) {
                buckets['Other'] = buckets['Other'] || [];
                buckets['Other'].push(skill);
            }
        });

        return Object.entries(buckets).map(([label, tags]) => `
            <div class="skill-group">
                <div class="skill-group-label">${esc(label)}</div>
                <div class="skill-tags">${tags.map(t => `<span class="skill-tag">${esc(t)}</span>`).join('')}</div>
            </div>`).join('');
    }

    // ── DOM injection helpers ─────────────────────────────────────

    /** Replace text content of all elements matching selector */
    function setText(selector, value) {
        document.querySelectorAll(selector).forEach(el => {
            el.textContent = value || '';
        });
    }

    /** Show or hide a section based on whether data has a value */
    function toggleSection(sectionId, show) {
        const el = document.getElementById(sectionId);
        if (el) el.style.display = show ? '' : 'none';
    }

    /** Set inner HTML of an element */
    function setHtml(selector, html) {
        const el = document.querySelector(selector);
        if (el) el.innerHTML = html;
    }

    /** Set href and text of a link element */
    function setLink(selector, value) {
        const el = document.querySelector(selector);
        if (el) {
            el.href = value || '#';
            el.textContent = value || '';
        }
    }

    // ─────────────────────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────────────────────

    function generate(profile) {

        // ── Header ────────────────────────────────────────────────
        setText('#cv-name', `${profile.firstName || ''} ${profile.lastName || ''}`);
        setText('#cv-email', profile.email);
        setText('#cv-phone', profile.phoneNumber);
        setText('#cv-location', `${profile.city || ''} ${profile.country || ''}`);

        const githubEl = document.getElementById('cv-github');
        if (githubEl) {
            if (hasValue(profile.gitHub)) {
                githubEl.style.display = '';
                githubEl.querySelector('a').href = profile.gitHub;
                githubEl.querySelector('a').textContent = profile.gitHub;
            } else {
                githubEl.style.display = 'none';
            }
        }

        const linkedInEl = document.getElementById('cv-linkedin');
        if (linkedInEl) {
            if (hasValue(profile.linkedIn)) {
                linkedInEl.style.display = '';
                linkedInEl.querySelector('a').href = profile.linkedIn;
                linkedInEl.querySelector('a').textContent = profile.linkedIn;
            } else {
                linkedInEl.style.display = 'none';
            }
        }

        // ── Education ─────────────────────────────────────────────
        const eduSection = document.getElementById('section-education');
        if (eduSection) {
            if (hasValue(profile.university)) {
                eduSection.style.display = '';
                setText('#cv-university', profile.university);
                setText('#cv-degree', profile.degree || '');
                setText('#cv-graduation',
                    profile.expectedGraduationYear ? profile.expectedGraduationYear : '');
                setText('#cv-university-start',
                    profile.universityStartYear ? profile.universityStartYear : '');
            } else {
                eduSection.style.display = 'none';
            }
        }

        // ── Work Experience ───────────────────────────────────────
        const workSection = document.getElementById('section-work');
        if (workSection) {
            if (hasValue(profile.workExperiences)) {
                workSection.style.display = '';
                setHtml('#work-list', renderWorkExperience(profile.workExperiences));
            } else {
                workSection.style.display = 'none';
            }
        }

        // ── Projects ──────────────────────────────────────────────
        const projectsSection = document.getElementById('section-projects');
        if (projectsSection) {
            if (hasValue(profile.projects)) {
                projectsSection.style.display = '';
                setHtml('#projects-list', renderProjects(profile.projects));
            } else {
                projectsSection.style.display = 'none';
            }
        }

        // ── Extracurricular ───────────────────────────────────────
        const extraSection = document.getElementById('section-extra');
        if (extraSection) {
            if (hasValue(profile.extraCurricularActivities)) {
                extraSection.style.display = '';
                setHtml('#extra-list', renderExtraCurricular(profile.extraCurricularActivities));
            } else {
                extraSection.style.display = 'none';
            }
        }


        // ── Skills ────────────────────────────────────────────────
        const skillsSection = document.getElementById('section-skills');
        if (skillsSection) {
            if (hasValue(profile.skills)) {
                skillsSection.style.display = '';
                setHtml('#skills-list', renderSkills(profile.skills));
            } else {
                skillsSection.style.display = 'none';
            }
        }
    }

    return { generate };

})();

window.CVGenerator = CVGenerator;  // Expose globally for WebView2 access