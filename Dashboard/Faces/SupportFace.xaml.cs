using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard.Faces
{
    // Help & support: open a ticket from the signed-in account, follow replies, reply, and join the
    // live chat when support offers one.
    public partial class SupportFace : ContentView
    {
        private List<SupportTicket> _tickets = new();
        private SupportTicket _selected;
        private bool _loading;

        public SupportFace()
        {
            InitializeComponent();
            foreach (var a in SupportService.DefaultAreas) AreaPicker.Items.Add(a);
        }

        // Called by the dashboard each time the section is shown.
        public async Task RefreshAsync(int? openTicketId = null)
        {
            SignedInLabel.Text = SessionService.Current != null
                ? $"Signed in as {SessionService.Current.Email}. Replies come to this address and appear here."
                : "Sign in to contact support.";
            await LoadTickets(openTicketId);
        }

        private async Task LoadTickets(int? openTicketId = null)
        {
            if (_loading) return;
            _loading = true;
            try
            {
                var (ok, error, tickets, areas) = await SupportService.GetMyTickets();
                if (!ok) { TicketsStatus.Text = error ?? "Couldn't reach SmartCube support."; return; }

                if (areas?.Length > 0 && !areas.SequenceEqual(AreaPicker.Items))
                {
                    var keep = AreaPicker.SelectedItem as string;
                    AreaPicker.Items.Clear();
                    foreach (var a in areas) AreaPicker.Items.Add(a);
                    if (keep != null) AreaPicker.SelectedItem = keep;
                }

                _tickets = tickets;
                TicketsStatus.Text = tickets.Count == 0 ? "No tickets yet. Use Contact support on the left if you need help." : "";
                BuildList();

                var invite = tickets.FirstOrDefault(t => t.ChatOffered);
                ChatBanner.IsVisible = invite != null;
                if (invite != null) { ChatBannerLabel.Text = $"{invite.Reference} · {invite.Subject}"; ChatBannerBtn.CommandParameter = invite.Id; }

                var reopen = openTicketId ?? _selected?.Id;
                if (reopen != null && tickets.Any(t => t.Id == reopen)) await OpenTicket(reopen.Value);
            }
            finally { _loading = false; }
        }

        private static (string Bg, string Fg) StatusColours(string status) => status switch
        {
            "Open" => ("#3B1A1A", "#FCA5A5"),
            "In progress" => ("#1E3A5F", "#93C5FD"),
            "Waiting on customer" => ("#3B2E1A", "#FCD34D"),
            _ => ("#0A3D2E", "#34D399"),
        };

        private void BuildList()
        {
            TicketList.Children.Clear();
            foreach (var t in _tickets)
            {
                var (bg, fg) = StatusColours(t.Status);
                var news = SupportService.HasNews(t);
                var status = new Border
                {
                    BackgroundColor = Color.FromArgb(bg), Stroke = Colors.Transparent, Padding = new Thickness(8, 2),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    VerticalOptions = LayoutOptions.Center,
                    Content = new Label { Text = t.Status, FontSize = 11, TextColor = Color.FromArgb(fg), FontAttributes = FontAttributes.Bold },
                };
                var info = new VerticalStackLayout { Spacing = 2 };
                info.Add(new Label { Text = t.Subject, FontSize = 13, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#F1F5F9"), LineBreakMode = LineBreakMode.TailTruncation });
                info.Add(new Label
                {
                    Text = $"{t.Reference} · {t.Area} · {(t.UpdatedAt ?? t.CreatedAt):dd MMM HH:mm}" + (t.ChatOffered ? " · live chat offered" : "") + (news ? " · new reply" : ""),
                    FontSize = 11, TextColor = Color.FromArgb(news ? "#60A5FA" : "#64748B"),
                });

                var grid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 8 };
                grid.Add(info, 0);
                grid.Add(status, 1);

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb(_selected?.Id == t.Id ? "#1E2D4A" : "#1C2744"),
                    Stroke = Color.FromArgb(news ? "#2563EB" : "#1E2D4A"), StrokeThickness = 1, Padding = new Thickness(12, 9),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Content = grid,
                };
                var id = t.Id;
                card.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => await OpenTicket(id)) });
                ToolTipProperties.SetText(card, "Open this ticket to read replies from support and answer them.");
                TicketList.Add(card);
            }
        }

        private async Task OpenTicket(int id)
        {
            var (ok, error, ticket, messages) = await SupportService.GetTicket(id);
            if (!ok) { ReplyStatus.Text = error; return; }
            _selected = ticket;
            var listed = _tickets.FirstOrDefault(t => t.Id == id);
            if (listed != null) SupportService.MarkSeen(listed);
            BuildList();

            DetailCard.IsVisible = true;
            DetailTitle.Text = $"{ticket.Reference} · {ticket.Subject}";
            DetailMeta.Text = $"{ticket.Area} · {ticket.Status} · opened {ticket.CreatedAt:dd MMM yyyy HH:mm}";
            JoinChatBtn.IsVisible = ticket.ChatOffered;
            ReplyBox.IsVisible = ReplyBtn.IsVisible = ticket.Status != "Closed";
            ReplyStatus.Text = ticket.Status == "Closed" ? "This ticket is closed. Open a new one if you still need help." : "";

            Thread.Children.Clear();
            foreach (var m in messages)
            {
                var bubble = new Border
                {
                    BackgroundColor = Color.FromArgb(m.IsStaff ? "#0F2A22" : "#1C2744"),
                    Stroke = Color.FromArgb(m.IsStaff ? "#14532D" : "#1E2D4A"), StrokeThickness = 1,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Padding = new Thickness(10, 8),
                    HorizontalOptions = m.IsStaff ? LayoutOptions.Start : LayoutOptions.End,
                    MaximumWidthRequest = 520,
                };
                var stack = new VerticalStackLayout { Spacing = 2 };
                stack.Add(new Label { Text = $"{(m.IsStaff ? "SmartCube Support" : "You")} · {m.CreatedAt:dd MMM HH:mm}", FontSize = 10, TextColor = Color.FromArgb("#64748B") });
                stack.Add(new Label { Text = m.Message, FontSize = 13, TextColor = Color.FromArgb("#E2E8F0"), LineBreakMode = LineBreakMode.WordWrap });
                bubble.Content = stack;
                Thread.Add(bubble);
            }
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            var area = AreaPicker.SelectedItem as string;
            var subject = SubjectEntry.Text?.Trim() ?? "";
            var message = MessageEditor.Text?.Trim() ?? "";
            SendStatus.IsVisible = true;
            SendStatus.TextColor = Color.FromArgb("#F87171");
            if (string.IsNullOrEmpty(area)) { SendStatus.Text = "Choose which area the problem is in."; return; }
            if (subject.Length < 3) { SendStatus.Text = "Give the ticket a short subject."; return; }
            if (message.Length < 10) { SendStatus.Text = "Describe the problem in a sentence or two."; return; }

            SendBtn.IsEnabled = false;
            SendStatus.TextColor = Color.FromArgb("#94A3B8");
            SendStatus.Text = "Sending…";
            var (ok, error, reference) = await SupportService.Create(area, subject, message);
            SendBtn.IsEnabled = true;
            if (!ok) { SendStatus.TextColor = Color.FromArgb("#F87171"); SendStatus.Text = error ?? "Couldn't send the ticket."; return; }

            SendStatus.TextColor = Color.FromArgb("#34D399");
            SendStatus.Text = $"Ticket {reference} sent. We've emailed you a copy and will reply here and by email.";
            SubjectEntry.Text = ""; MessageEditor.Text = ""; AreaPicker.SelectedIndex = -1;
            _selected = null;
            await LoadTickets();
        }

        private async void OnReplyClicked(object sender, EventArgs e)
        {
            if (_selected == null) return;
            var text = ReplyEditor.Text?.Trim() ?? "";
            if (text.Length < 2) { ReplyStatus.Text = "Type a reply first."; return; }
            ReplyBtn.IsEnabled = false;
            ReplyStatus.Text = "Sending…";
            var (ok, error) = await SupportService.Reply(_selected.Id, text);
            ReplyBtn.IsEnabled = true;
            if (!ok) { ReplyStatus.Text = error ?? "Couldn't send the reply."; return; }
            ReplyEditor.Text = "";
            ReplyStatus.Text = "Reply sent.";
            await LoadTickets(_selected.Id);
        }

        private async void OnJoinChatClicked(object sender, EventArgs e)
        {
            if (_selected?.ChatOffered == true) await OpenChat(_selected);
        }

        private async void OnBannerJoinClicked(object sender, EventArgs e)
        {
            var t = _tickets.FirstOrDefault(x => x.Id == (ChatBannerBtn.CommandParameter as int? ?? 0)) ?? _tickets.FirstOrDefault(x => x.ChatOffered);
            if (t != null) await OpenChat(t);
        }

        private async Task OpenChat(SupportTicket t)
        {
            SupportService.MarkSeen(t);
            var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
            if (nav != null) await nav.PushAsync(new SupportChatPage(t.ChatUrl, t.Reference, t.Subject));
        }

        private async void OnRefreshClicked(object sender, EventArgs e) => await LoadTickets();

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null) await page.DisplayAlert("Help & support",
                "Send the SmartCube support team a message from your account: choose the area, give it a subject and describe what happened. " +
                "Your tickets are listed on the right; open one to read replies from support and answer them. " +
                "When support picks up your ticket they may invite you to a live chat, which opens inside the app. " +
                "Tickets and chats are kept on the SmartCube server so support can see them; copies of replies are emailed to you.",
                "OK");
        }
    }
}
