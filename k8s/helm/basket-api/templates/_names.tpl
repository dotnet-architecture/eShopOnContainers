{{- define "suffix-name" -}}
{{- if .Values.app.name -}}
{{- .Values.app.name -}}
{{- else -}}
{{- .Release.Name -}}
{{- end -}}
{{- end -}}

{{- define "sql-name" -}}
{{- if .Values.inf.sql.host -}}
{{- .Values.inf.sql.host -}}
{{- else -}}
{{- printf "%s" "sql-data" -}}
{{- end -}}
{{- end -}}

{{- define "url-identity" -}}
{{- if .Values.app.ingress.suffix -}}
{{- $suffix := include "suffix-name" . -}}
{{- printf "%s/identity-api-%s" .Values.inf.k8s.dns $suffix -}}
{{- else -}}
{{- printf "%s/identity-api" .Values.inf.k8s.dns -}}
{{- end -}}
{{- end -}}

{{ define "pathBase" -}}
{{- if .Values.app.ingress.suffix -}}
{{- $suffix := include "suffix-name" . -}}
{{- printf "%s-%s"  .Values.pathBase $suffix -}}
{{- else -}}
{{- .Values.pathBase -}}
{{- end -}}
{{- end -}}