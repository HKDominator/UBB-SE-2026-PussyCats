using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Repositories
{
    public class SkillGroupRepository : ISkillGroupRepository
    {
        private List<SkillGroup> skillGroups;

        public SkillGroupRepository()
        {
            skillGroups = new List<SkillGroup>();
            LoadSkillGroups();
        }

        private void LoadSkillGroups()
        {
            // Frontend Developer
            skillGroups.Add(new SkillGroup { GroupName = "UI Markup", Skills = new List<string> { "HTML", "CSS", "SCSS", "Tailwind" }, Weight = 20, JobRole = JobRole.FrontendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "JavaScript", Skills = new List<string> { "JavaScript", "TypeScript" }, Weight = 20, JobRole = JobRole.FrontendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Frontend Framework", Skills = new List<string> { "React", "Angular", "Vue.js", "Svelte" }, Weight = 25, JobRole = JobRole.FrontendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Version Control", Skills = new List<string> { "Git", "GitHub" }, Weight = 10, JobRole = JobRole.FrontendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Testing", Skills = new List<string> { "Jest", "Cypress", "Selenium" }, Weight = 10, JobRole = JobRole.FrontendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Build Tools", Skills = new List<string> { "Webpack", "Vite", "Parcel" }, Weight = 8, JobRole = JobRole.FrontendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Design Collaboration", Skills = new List<string> { "Figma", "Adobe XD", "Zeplin" }, Weight = 7, JobRole = JobRole.FrontendDeveloper });

            // Backend Developer
            skillGroups.Add(new SkillGroup { GroupName = "Backend Language", Skills = new List<string> { "Java", "C#", "Python", "Node.js", "Go" }, Weight = 25, JobRole = JobRole.BackendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Web Framework", Skills = new List<string> { "Spring Boot", "ASP.NET", "Django" }, Weight = 20, JobRole = JobRole.BackendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Database Management", Skills = new List<string> { "SQL", "PostgreSQL", "MySQL", "MongoDB", "Redis" }, Weight = 20, JobRole = JobRole.BackendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "API Design", Skills = new List<string> { "REST", "GraphQL", "gRPC" }, Weight = 15, JobRole = JobRole.BackendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Version Control", Skills = new List<string> { "Git", "GitHub" }, Weight = 10, JobRole = JobRole.BackendDeveloper });
            skillGroups.Add(new SkillGroup { GroupName = "Testing", Skills = new List<string> { "JUnit", "NUnit", "pytest", "Postman" }, Weight = 10, JobRole = JobRole.BackendDeveloper });

            // UI/UX Designer
            skillGroups.Add(new SkillGroup { GroupName = "Design Tools", Skills = new List<string> { "Figma", "Adobe XD", "Sketch", "InVision" }, Weight = 30, JobRole = JobRole.UIUXDesigner });
            skillGroups.Add(new SkillGroup { GroupName = "Prototyping", Skills = new List<string> { "Figma Prototyping", "Marvel", "Axure" }, Weight = 20, JobRole = JobRole.UIUXDesigner });
            skillGroups.Add(new SkillGroup { GroupName = "User Research", Skills = new List<string> { "Interviews", "Surveys", "Usability Testing" }, Weight = 20, JobRole = JobRole.UIUXDesigner });
            skillGroups.Add(new SkillGroup { GroupName = "Visual Design", Skills = new List<string> { "Typography", "Color Theory", "Grid Systems" }, Weight = 15, JobRole = JobRole.UIUXDesigner });
            skillGroups.Add(new SkillGroup { GroupName = "Handoff", Skills = new List<string> { "Zeplin", "Figma", "Storybook" }, Weight = 10, JobRole = JobRole.UIUXDesigner });
            skillGroups.Add(new SkillGroup { GroupName = "Analytics", Skills = new List<string> { "Google Analytics", "Hotjar", "Mixpanel" }, Weight = 5, JobRole = JobRole.UIUXDesigner });

            // DevOps Engineer
            skillGroups.Add(new SkillGroup { GroupName = "Containerization", Skills = new List<string> { "Docker", "Podman" }, Weight = 20, JobRole = JobRole.DevOpsEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Orchestration", Skills = new List<string> { "Kubernetes", "Docker Swarm", "OpenShift" }, Weight = 20, JobRole = JobRole.DevOpsEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "CI/CD", Skills = new List<string> { "Jenkins", "GitHub Actions", "GitLab CI", "CircleCI" }, Weight = 20, JobRole = JobRole.DevOpsEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Cloud Platform", Skills = new List<string> { "AWS", "Azure", "Google Cloud" }, Weight = 15, JobRole = JobRole.DevOpsEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Infrastructure as Code", Skills = new List<string> { "Terraform", "Ansible", "Pulumi" }, Weight = 15, JobRole = JobRole.DevOpsEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Monitoring", Skills = new List<string> { "Prometheus", "Grafana", "Datadog" }, Weight = 10, JobRole = JobRole.DevOpsEngineer });

            // Project Manager
            skillGroups.Add(new SkillGroup { GroupName = "Methodologies", Skills = new List<string> { "Scrum", "Kanban", "Agile", "Waterfall" }, Weight = 25, JobRole = JobRole.ProjectManager });
            skillGroups.Add(new SkillGroup { GroupName = "Project Tools", Skills = new List<string> { "Jira", "Trello", "Asana" }, Weight = 20, JobRole = JobRole.ProjectManager });
            skillGroups.Add(new SkillGroup { GroupName = "Risk Management", Skills = new List<string> { "Risk Assessment", "Mitigation Planning" }, Weight = 20, JobRole = JobRole.ProjectManager });
            skillGroups.Add(new SkillGroup { GroupName = "Communication", Skills = new List<string> { "Stakeholder Management", "Reporting", "Presentations" }, Weight = 20, JobRole = JobRole.ProjectManager });
            skillGroups.Add(new SkillGroup { GroupName = "Budgeting", Skills = new List<string> { "Cost Estimation", "Budget Tracking", "MS Project" }, Weight = 15, JobRole = JobRole.ProjectManager });

            // Data Analyst
            skillGroups.Add(new SkillGroup { GroupName = "Query Language", Skills = new List<string> { "SQL", "PostgreSQL", "BigQuery" }, Weight = 25, JobRole = JobRole.DataAnalyst });
            skillGroups.Add(new SkillGroup { GroupName = "Data Visualization", Skills = new List<string> { "Power BI", "Tableau", "Looker" }, Weight = 25, JobRole = JobRole.DataAnalyst });
            skillGroups.Add(new SkillGroup { GroupName = "Programming", Skills = new List<string> { "Python", "R" }, Weight = 20, JobRole = JobRole.DataAnalyst });
            skillGroups.Add(new SkillGroup { GroupName = "Statistical Analysis", Skills = new List<string> { "Descriptive Statistics", "Regression", "Hypothesis Testing" }, Weight = 15, JobRole = JobRole.DataAnalyst });
            skillGroups.Add(new SkillGroup { GroupName = "Spreadsheets", Skills = new List<string> { "Excel", "Google Sheets" }, Weight = 10, JobRole = JobRole.DataAnalyst });
            skillGroups.Add(new SkillGroup { GroupName = "Data Cleaning", Skills = new List<string> { "Pandas", "OpenRefine" }, Weight = 5, JobRole = JobRole.DataAnalyst });

            // Cybersecurity Specialist
            skillGroups.Add(new SkillGroup { GroupName = "Network Security", Skills = new List<string> { "Firewalls", "VPN", "IDS/IPS", "TCP/IP" }, Weight = 20, JobRole = JobRole.CybersecuritySpecialist });
            skillGroups.Add(new SkillGroup { GroupName = "Penetration Testing", Skills = new List<string> { "Metasploit", "Burp Suite", "Nmap" }, Weight = 20, JobRole = JobRole.CybersecuritySpecialist });
            skillGroups.Add(new SkillGroup { GroupName = "SIEM & Monitoring", Skills = new List<string> { "Splunk", "IBM QRadar", "Microsoft Sentinel" }, Weight = 15, JobRole = JobRole.CybersecuritySpecialist });
            skillGroups.Add(new SkillGroup { GroupName = "Cryptography", Skills = new List<string> { "AES", "RSA", "PKI", "TLS/SSL" }, Weight = 15, JobRole = JobRole.CybersecuritySpecialist });
            skillGroups.Add(new SkillGroup { GroupName = "Compliance & Standards", Skills = new List<string> { "ISO 27001", "GDPR", "NIST", "SOC 2" }, Weight = 15, JobRole = JobRole.CybersecuritySpecialist });
            skillGroups.Add(new SkillGroup { GroupName = "Incident Response", Skills = new List<string> { "Forensics", "Malware Analysis", "DFIR" }, Weight = 15, JobRole = JobRole.CybersecuritySpecialist });

            // AI/ML Engineer
            skillGroups.Add(new SkillGroup { GroupName = "ML Frameworks", Skills = new List<string> { "TensorFlow", "PyTorch", "scikit-learn", "Keras" }, Weight = 25, JobRole = JobRole.AIMLEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Programming", Skills = new List<string> { "Python", "R", "Julia" }, Weight = 20, JobRole = JobRole.AIMLEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Mathematics", Skills = new List<string> { "Linear Algebra", "Calculus", "Probability", "Statistics" }, Weight = 20, JobRole = JobRole.AIMLEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Data Engineering", Skills = new List<string> { "Pandas", "NumPy", "Apache Spark", "SQL" }, Weight = 15, JobRole = JobRole.AIMLEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "Model Deployment", Skills = new List<string> { "Docker", "FastAPI", "MLflow" }, Weight = 10, JobRole = JobRole.AIMLEngineer });
            skillGroups.Add(new SkillGroup { GroupName = "NLP / Computer Vision", Skills = new List<string> { "Hugging Face", "OpenCV", "NLTK", "spaCy" }, Weight = 10, JobRole = JobRole.AIMLEngineer });
        }

        public List<SkillGroup> GetSkillsGroupByRole(JobRole role)
        {
            List<SkillGroup> result = new List<SkillGroup>();
            foreach (SkillGroup group in skillGroups)
            {
                if (group.JobRole == role)
                {
                    result.Add(group);
                }
            }
            return result;
        }
    }
}
